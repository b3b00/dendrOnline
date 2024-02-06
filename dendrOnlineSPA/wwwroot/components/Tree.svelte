<script lang="ts">

    import {repository, tree, setTree, loadedNotes} from '../scripts/dendronStore.js';
    import { onMount, getContext } from 'svelte';    
    import { DendronClient} from "../scripts/dendronClient.js";    
    import TreeView from "@bolduh/svelte-treeview";    
    import NoteNodeWraper from "./NoteNodeWraper.svelte";
    import {Dendron, Node, Repository} from '../scripts/types'
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
        <TreeView emptyTreeMessage="nothing to show...Maybe your {$repository.name} repository is not a dendron repository" root={currentTree} nodeTemplate={NoteNodeWraper} filter={nodefilter} searchPlaceholder="search the notes ..."></TreeView>
    <!--{:catch error}-->
    <!--    <p style="color: red">{error.message}</p>-->
    <!--{/await}-->
</div>