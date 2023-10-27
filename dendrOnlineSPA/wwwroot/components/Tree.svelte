<script>

    import {repositories, repository} from '../scripts/dendronStore.js';
    import { onMount } from 'svelte';
    import NoteNode from "./NoteNode.svelte";
    import { DendronClient} from "../scripts/dendronClient.js";
    import TreeNode from "./TreeNode.svelte";
    import TreeView from "./TreeView.svelte";

    export let params = {}
    
    let currentRepository = {};
    
    let currentTree = {};
    
    let childAccessor = (x) => {
        if (x.child != undefined && x.child != null && Array.isArray(x.child)) {
            return x.child;
        }
        return [];
    }
    
    onMount(async () => {
        currentRepository = $repository; 
        let tree = await DendronClient.GetTree(currentRepository.id);
        console.log(tree);
        currentTree = tree;
    });
    
</script>
<div>
    <a href="#/">home</a>
    <TreeView root={currentTree} childAccessor={childAccessor} nodeTemplate={NoteNode} filter={(x) => x}></TreeView>
</div>