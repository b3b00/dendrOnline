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

    export let id:string;

    export let refresh:string = undefined;

    let currentRepository : Repository = undefined;
    
    let currentTree : Node = undefined;

    let nodefilter = (node, search)  => {
		const contains = search === undefined || search === null || search.length== 0 || node.name.toLocaleLowerCase().includes(search.toLocaleLowerCase());       
		return contains;
	};

    const fuseNoteFilter = (node:Node, filter:NoteFilter) : boolean => {    
        testFuse(filter);
        let note = getLoadedNote(node.id);        
        if (filter.filter !== undefined && filter.filter !== null && filter.filter.length > 0) {                
            // TODO use fuse
            if(node.name.toLocaleLowerCase().includes(filter.filter)) {
                return true;
            };
            if (filter.searchInNotes && note) {
                // TODO use fuse
                if(note.body.toLocaleLowerCase().includes(filter.filter)) {
                    return true;
                }                
            }
            return false;
        }
        return true;
    }


    const testFuse = (filter:NoteFilter):void =>  {
        const notes = $loadedNotes;
        const fuseOptions = {
            isCaseSensitive : false,
            includeScore: true,
            keys:['body'],
            minScore: 70,
            limit:1000
        }
        const fuse = new Fuse(notes);
        const result = fuse.search(filter.filter,fuseOptions);
        console.log(result);
    }

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

</script>
<div>
    <!--{#await currentTree}-->
    <!--    <p>...loading note tree...</p>-->
    <!--{:then t}-->
        <Accordion tab="25px" searchTemplate={NoteFilterTemplate} complexFilter={fuseNoteFilter} disposition="left" emptyTreeMessage="nothing to show...Maybe your {$repository.name} repository is not a dendron repository" root={currentTree} nodeTemplate={NoteNodeWraper} searchPlaceholder="search the notes ..."></Accordion>
    <!--{:catch error}-->
    <!--    <p style="color: red">{error.message}</p>-->
    <!--{/await}-->
</div>