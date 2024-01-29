<script lang="ts">
    import {setRepository, repositories, setRepositories} from '../scripts/dendronStore.js';
    import {push} from 'svelte-spa-router'
    import {onMount} from 'svelte';
    import {DendronClient} from "../scripts/dendronClient.js";
    import {Repository} from "../scripts/types";

    let allRepositories: Repository[] = [];

    let filteredRepositories: Repository[] = [];

    let filter:string = "";

    $:{
        allRepositories = $repositories;
        filter = filter;
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
    <input type="text" bind:value={filter}/>
    {#if filteredRepositories.length > 0}
        {#each filteredRepositories as repository}
            <div class="w3-display-container" aria-hidden="true" style="cursor: pointer" on:click={() => {                            
                            setRepository(repository);
                            push(`#/tree/${repository.id}`);
                    }}>
                {repository.name}
            </div>
        {/each}
    {/if}

</div>