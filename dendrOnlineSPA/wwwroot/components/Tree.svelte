<script>

    import {repository, tree, setTree} from '../scripts/dendronStore.js';
    import { onMount } from 'svelte';    
    import { DendronClient} from "../scripts/dendronClient.js";    
    import TreeView from "@bolduh/svelte-treeview";    
    import NoteNodeWraper from "./NoteNodeWraper.svelte";
    
    
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
        currentTree = $tree;
    }

    onMount(async () => {
        currentRepository = $repository;
        currentTree = $tree;
        if (currentTree === null || currentTree === undefined || currentTree == {} || !currentTree.hasOwnProperty('name')) {
            currentTree = await DendronClient.GetTree(currentRepository.id);
            setTree(currentTree);
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