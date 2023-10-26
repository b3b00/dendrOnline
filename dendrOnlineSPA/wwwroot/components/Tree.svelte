<script>

    import {repositories, repository} from '../scripts/dendronStore.js';
    import { onMount } from 'svelte';
    import { DendronClient} from "../scripts/dendronClient.js";
    import TreeNode from "./TreeNode.svelte";

    let currentRepository = {};
    
    let currentTree = {};
    
    onMount(async () => {
        currentRepository = $repository; 
        let tree = await DendronClient.GetTree(currentRepository.id);
        console.log(tree);
        currentTree = tree;
    });
    
</script>
<div>
    <a href="#/">home</a>
    <TreeNode node="{currentTree}"/>
</div>