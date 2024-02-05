<script lang="ts">

    import {repository, tree, setTree} from '../scripts/dendronStore.js';
    import { onMount, getContext } from 'svelte';    
    import { DendronClient} from "../scripts/dendronClient.js";    
    import TreeView from "@bolduh/svelte-treeview";    
    import NoteNodeWraper from "./NoteNodeWraper.svelte";
    import {Node, Repository} from '../scripts/types'
    import ErrorDialog from './ErrorDialog.svelte';
    import type { Context } from 'svelte-simple-modal';
    
    const modal = getContext<Context>('simple-modal');

    export let id:string;

    export let refresh:string = undefined;

    let currentRepository : Repository = undefined;
    
    let currentTree : Node = undefined;
    
    let childAccessor = (x) => {
        if (x!= null && x !== undefined && x.child != undefined && x.child != null && Array.isArray(x.child)) {
            return x.child;
        }
        return [];
    }

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
            const newCurrentTree = await DendronClient.GetTree(currentRepository.id);
            if (newCurrentTree.isOk) {
                currentTree = newCurrentTree.theResult
                setTree(currentTree);
            }
            else {
                modal.open(
                    ErrorDialog,
                    {
                        message: `Une erreur est survenue: ${newCurrentTree.errorMessage} `,                                                
                        closeButton: true,
                        closeOnEsc: true,
                        closeOnOuterClick: true,
                    });
            }
            

        }
    });

</script>
<div>
    <!--{#await currentTree}-->
    <!--    <p>...loading note tree...</p>-->
    <!--{:then t}-->
        <TreeView emptyTreeMessage="y a que dalle !" root={currentTree} nodeTemplate={NoteNodeWraper} filter={nodefilter}></TreeView>
    <!--{:catch error}-->
    <!--    <p style="color: red">{error.message}</p>-->
    <!--{/await}-->
</div>