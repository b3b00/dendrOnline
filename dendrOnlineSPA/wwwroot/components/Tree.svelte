<script>

    import {repositories, repository, tree, setTree} from '../scripts/dendronStore.js';
    import { onMount } from 'svelte';
    import NoteNode from "./NoteNode.svelte";
    import { DendronClient} from "../scripts/dendronClient.js";
    import TreeNode from "./TreeNode.svelte";
    import TreeView from "./TreeView.svelte";
    
    
    let currentRepository = {};
    
    let currentTree = {};
    
    let childAccessor = (x) => {
        if (x.child != undefined && x.child != null && Array.isArray(x.child)) {
            return x.child;
        }
        return [];
    }
    
    $: {
        currentTree = currentTree;
    }
    
    onMount(async () => {
        
        
        currentRepository = $repository;
        currentTree = $tree;
        if (currentTree === null || currentTree === undefined || currentTree == {} || !currentTree.hasOwnProperty('name')) {            
            currentTree = await DendronClient.GetTree(currentRepository.id);
            setTree(currentTree);
        }
        console.log(tree);

    });
    
</script>
<div>
i{#if (currentTree) }
    <TreeView root={currentTree} childAccessor={childAccessor} nodeTemplate={NoteNode} filter={(x) => x}></TreeView>
    {:else}
    <i>Loading...</i>
    {/if}
</div>