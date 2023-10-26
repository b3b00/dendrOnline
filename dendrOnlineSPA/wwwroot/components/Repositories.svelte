
<script>
    import {setRepository, repositories, setRepositories} from '../scripts/dendronStore.js';
    import {push, pop, replace} from 'svelte-spa-router'
    import { onMount } from 'svelte';

    let allRepositories = [];
    
    let filteredRepositories = [];

    onMount(async () => {
        const res = await fetch('/repositories');
        if (res.status >= 200 && res.status <= 299) {            
            allRepositories = await res.json()
            setRepository(allRepositories);
            filteredRepositories = allRepositories;
        } else {
            let body = await res.json();
        }
    });
    
</script>

<div>
    {#if filteredRepositories.length > 0}
        {#each filteredRepositories as repository}            
                <li class="w3-display-container">
                    
                    <span style="cursor: pointer" on:click={() => {                            
                            setRepository(repository);
                            push("#/tree/{repository.id}");
                    }}>
                    {repository.id} - {repository.name}
                    </span>
                </li>            
        {/each}
    {/if}
    
</div>