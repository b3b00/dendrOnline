<script>

    import {repositories, repository, tree, setTree} from '../scripts/dendronStore.js';
    import { onMount } from 'svelte';
    import NoteNode from "./NoteNode.svelte";
    import { DendronClient} from "../scripts/dendronClient.js";    
    import TreeView from "@bolduh/svelte-treeview";
    
    
    let currentRepository = {};
    
    let currentTree = {};
    
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
        currentTree = currentTree;
    }

    onMount(async () => {
        currentRepository = $repository;
        currentTree = $tree;
        if (currentTree === null || currentTree === undefined || currentTree == {} || !currentTree.hasOwnProperty('name')) {
            currentTree = DendronClient.GetTree(currentRepository.id);
            setTree(currentTree);
        }
    });

</script>
<div>
    {#await currentTree}
        <p>...loading note tree...</p>
    {:then t}
        {@debug t}
        <TreeView emptyTreeMessage="y a que dalle !" root={t} nodeTemplate={NoteNode} filter={nodefilter}></TreeView>
    {:catch error}
        <p style="color: red">{error.message}</p>
    {/await}
</div>