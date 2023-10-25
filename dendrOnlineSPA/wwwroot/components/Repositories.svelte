
<script>
    import {repository, setRepository, repositories, setRepositories} from '../scripts/dendronStore.js';
    import { onMount } from 'svelte';

    

    onMount(async () => {
        const res = await fetch('/repositories');
        if (res.status >= 200 && res.status <= 299) {
            $repositories = await res.json();
            
        } else {
            let body = await res.json();
        }
    });
    
</script>

<div>
    {#if $repositories.length > 0}
        {#each $repositories as repository}            
                <li class="w3-display-container">
                    
                    <a href="/notes/{repository.id} " on:click={setRepository(repository)}>
                    {repository.id} - {repository.name}
                    </a>
                </li>            
        {/each}
    {/if}
    
</div>