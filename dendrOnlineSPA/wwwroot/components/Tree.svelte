<script lang="ts">

    import {repository, tree, setTree, loadedNotes, getLoadedNote} from '../scripts/dendronStore.js';
    import { onMount, getContext } from 'svelte';    
    import { DendronClient} from "../scripts/dendronClient.js";        
    import Accordion from "@bolduh/svelte-nested-accordion/src/Accordion.svelte";
    import NoteNodeWraper from "./NoteNodeWraper.svelte";
    import {Dendron, Node, NoteFilter, Repository} from '../scripts/types'
    import NoteFilterTemplate from './TreeFilter.svelte';
    import { Wave } from 'svelte-loading-spinners';

    
    import ErrorDialog from './ErrorDialog.svelte';
    import type { Context } from 'svelte-simple-modal';
    
    const modal = getContext<Context>('simple-modal');

    export const id:string = '';

    export let refresh:string = undefined;

    let currentRepository : Repository = undefined;
    
    let currentTree : Node = undefined;

    let loading:boolean = false;

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
            loading = true;
            const dendron = await DendronClient.GetDendron(currentRepository.id);
            loading = false;
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
    {#if loading}
        <div class="spinner-item" title="Wave">
            <Wave  size="45" />
            <div class="spinner-title">Dendron is loading...</div>
        </div>
    {:else}
        <Accordion tab="25px" disposition="left" emptyTreeMessage="nothing to show..." root={currentTree} nodeTemplate={NoteNodeWraper} searchTemplate={NoteFilterTemplate} complexFilter={noteFilter} nodeClass="dendron">
            <style slot="style">
                .dendron {
                    border-bottom: thin solid black;
                    border-left: thin dotted black;
                    padding : 10px
                }

                .dendron:hover {
                    background-color:lightgrey
                }
            </style>
        </Accordion>
    {/if}
</div>