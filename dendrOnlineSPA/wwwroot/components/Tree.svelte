<script>

    import {repositories, repository} from '../scripts/dendronStore.js';
    import { onMount } from 'svelte';
    
    let currentRepository = {};

    onMount(async () => {
        currentRepository = $repository; 
    });
    
</script>
<div>
    <a href="#/">home</a>
    tree for <b>{currentRepository.name}</b> <i>[{currentRepository.id}]</i> to come
</div>