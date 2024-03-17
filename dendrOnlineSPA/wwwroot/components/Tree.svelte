<script lang="ts">

    import {repository, tree, setTree, loadedNotes, getLoadedNote} from '../scripts/dendronStore.js';
    import { onMount, getContext } from 'svelte';    
    import { DendronClient} from "../scripts/dendronClient.js";        
    import Accordion from "@bolduh/svelte-nested-accordion/src/Accordion.svelte";
    import NoteNodeWraper from "./NoteNodeWraper.svelte";
    import {Dendron, Node, NoteFilter, Repository} from '../scripts/types'
    import TreeFilter from './TreeFilter.svelte';
    
    import ErrorDialog from './ErrorDialog.svelte';
    import type { Context } from 'svelte-simple-modal';
    
    const modal = getContext<Context>('simple-modal');

    export let id:string;

    export let refresh:string = undefined;

    let currentRepository : Repository = undefined;
    
    let currentTree : Node = undefined;

    $: {
        currentTree = $tree;
    }

    

    onMount(async () => {
        currentRepository = $repository;
        currentTree = $tree;
        if (currentTree === null || currentTree === undefined || !currentTree.hasOwnProperty('name') || refresh) {
            const dendron = await DendronClient.GetDendron(currentRepository.id);
            if (dendron.isOk) {
                setTree(dendron.theResult.hierarchy);
                currentTree = dendron.theResult.hierarchy;
                $loadedNotes = dendron.theResult.notes;
                console.log(`dendron is fully loaded`,dendron);
            }            
            else {
                modal.open(
                    ErrorDialog,
                    {
                        message: `An error occured: ${dendron.errorMessage} `
                    },
                    {
                        closeButton: true,
                        closeOnEsc: true,
                        closeOnOuterClick: true
                    });
            }            
        }

  
        });

        const noteFilter = (node:Node, filter:NoteFilter) : boolean => {    
            console.log(`note filter ${filter.filter} ${filter.searchInNotes} - ${node.id}`)
            // todo use fuse
            let note = getLoadedNote(node.id);
            console.log(note);
            if (filter.filter !== undefined && filter.filter !== null && filter.filter.length > 0) {                
                if(node.name.toLocaleLowerCase().includes(filter.filter)) {
                    return true;
                };
                if (note) {
                    const x = note.body.toLocaleLowerCase().includes(filter.filter);
                    return x;
                }
                return false;
            }
            return true;
        }

</script>
<div>
    <!--{#await currentTree}-->
    <!--    <p>...loading note tree...</p>-->
    <!--{:then t}-->
        <Accordion tab="25px" searchTemplate={TreeFilter} complexFilter={noteFilter} disposition="left" emptyTreeMessage="nothing to show...Maybe your {$repository.name} repository is not a dendron repository" root={currentTree} nodeTemplate={NoteNodeWraper} searchPlaceholder="search the notes ..."></Accordion>
    <!--{:catch error}-->
    <!--    <p style="color: red">{error.message}</p>-->
    <!--{/await}-->
</div>