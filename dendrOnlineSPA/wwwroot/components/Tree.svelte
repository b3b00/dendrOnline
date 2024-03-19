<script lang="ts">

    import {repository, tree, setTree, loadedNotes, getLoadedNote} from '../scripts/dendronStore.js';
    import { onMount, getContext } from 'svelte';    
    import { DendronClient} from "../scripts/dendronClient.js";        
    import Accordion from "@bolduh/svelte-nested-accordion/src/Accordion.svelte";
    import NoteNodeWraper from "./NoteNodeWraper.svelte";
    import {Dendron, Node, NoteFilter, Repository} from '../scripts/types'
    import NoteFilterTemplate from './TreeFilter.svelte';
    import Fuse from 'fuse.js';

    
    import ErrorDialog from './ErrorDialog.svelte';
    import type { Context } from 'svelte-simple-modal';
    
    const modal = getContext<Context>('simple-modal');

    export const id:string = '';

    export let refresh:string = undefined;

    let currentRepository : Repository = undefined;
    
    let currentTree : Node = undefined;

 

    const noteFilter = (node:Node, filter:NoteFilter) : boolean => {
        let note = getLoadedNote(node.id);
        if (filter.filter !== undefined && filter.filter !== null && filter.filter.length > 0) {
            // TODO use fuse
            if(node.name.toLocaleLowerCase().includes(filter.filter)) {
                return true;
            };
            if (filter.searchInNotes && note) {
                if(note.body.toLocaleLowerCase().includes(filter.filter)) {
                    return true;
                }                
            }
            return false;
        }
        return true;
    }



    $: {
        console.log('reactive statement : refreshing tree',$tree);
        currentTree = $tree;
    }

    

    onMount(async () => {
        currentRepository = $repository;
        currentTree = $tree;
        if (currentTree === null || currentTree === undefined || !currentTree.hasOwnProperty('name') || refresh) {
            console.log('loading tree from BackEnd')
            const dendron = await DendronClient.GetDendron(currentRepository.id);
            console.log('Backend response is ',dendron);
            if (dendron.isOk) {
                console.log('setting tree and notes in store ')
                $tree = dendron.theResult.hierarchy;
                //setTree(dendron.theResult.hierarchy);
                currentTree = $tree;
                $loadedNotes = dendron.theResult.notes;
                console.log(`dendron is now fully loaded`,dendron);
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
        else {
            console.log('tree loaded from store',currentTree);
        }
    });

</script>
<div>
    <!--{#await currentTree}-->
    <!--    <p>...loading note tree...</p>-->
    <!--{:then t}-->
    <Accordion tab="25px" disposition="left" emptyTreeMessage="nothing to show...Maybe your {$repository.name} repository is not a dendron repository" root={currentTree} nodeTemplate={NoteNodeWraper} searchTemplate={NoteFilterTemplate} complexFilter={noteFilter}></Accordion>
        
    <!--{:catch error}-->
    <!--    <p style="color: red">{error.message}</p>-->
    <!--{/await}-->
</div>