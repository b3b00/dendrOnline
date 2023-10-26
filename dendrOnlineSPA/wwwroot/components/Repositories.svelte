
<script>
    import {setRepository, repositories, setRepositories} from '../scripts/dendronStore.js';
    import {push, pop, replace} from 'svelte-spa-router'
    import { onMount } from 'svelte';
    import {DendronClient} from "../scripts/dendronClient.js";

    let allRepositories = [];
    
    let filteredRepositories = [];

    onMount(async () => {

        let allRepositories = await DendronClient.GetRepositories();
        setRepositories(allRepositories);
        filteredRepositories = allRepositories;
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