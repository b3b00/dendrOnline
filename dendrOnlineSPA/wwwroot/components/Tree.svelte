<script>

    import {repositories, repository} from '../scripts/dendronStore.js';
    import { onMount } from 'svelte';
    import { DendronClient} from "../scripts/dendronClient.js";

    let currentRepository = {};
    
    let currentTree = {};
    
    onMount(async () => {
        currentRepository = $repository; 
        let tree = await DendronClient.GetTree(currentRepository.id);
        currentTree = tree;
    });
    
</script>
<div>
    <a href="#/">home</a>
    tree for <b>{currentRepository.name}</b> <i>[{currentRepository.id}]</i> to come
</div>