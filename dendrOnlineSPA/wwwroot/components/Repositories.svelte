
<script>
    import {setRepository, repositories, setRepositories} from '../scripts/dendronStore.js';
    import {push} from 'svelte-spa-router'
    import { onMount } from 'svelte';
    import {DendronClient} from "../scripts/dendronClient.js";

    let allRepositories = [];
    
    let filteredRepositories = [];

    let filter = "";
    
    $:{
        allRepositories = $repositories;
        filter=filter;
        filteredRepositories = allRepositories.filter(
            x => x.name.toLowerCase().includes(filter.toLowerCase())
        );
    }
    
    onMount(async () => {

        let allRepositories = await DendronClient.GetRepositories();
        setRepositories(allRepositories);
        filteredRepositories = allRepositories;
    });
    
</script>

<div>
    <input type="text" bind:value = {filter}/>
    {#if filteredRepositories.length > 0}
        {#each filteredRepositories as repository}            
                <li class="w3-display-container">
                    
                    <span style="cursor: pointer" on:click={() => {                            
                            setRepository(repository);
                            push(`#/tree/${repository.id}`);
                    }}>
                    {repository.id} - {repository.name}
                    </span>
                </li>            
        {/each}
    {/if}
    
</div>