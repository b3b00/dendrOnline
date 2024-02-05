<script lang="ts">
    import {setRepository, repositories, setRepositories} from '../scripts/dendronStore.js';
    import {push} from 'svelte-spa-router'
    import {onMount, getContext} from 'svelte';
    import {DendronClient} from "../scripts/dendronClient.js";
    import {Repository } from "../scripts/types";
    import type { Context } from 'svelte-simple-modal';
  import ErrorDialog from './ErrorDialog.svelte';

    const modal = getContext<Context>('simple-modal');

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
        console.log(`repos.svelte -> `,allRepositories);
        if (allRepositories.isOk) {
            setRepositories(allRepositories.theResult);
            filteredRepositories = allRepositories.theResult;
        }
        else {
            console.log(`error while getting repos ${allRepositories.errorMessage}`);
            modal.open(
                ErrorDialog,
                {
                    message: `Une erreur est survenue: ${allRepositories.errorMessage} `,                                                
                    closeButton: true,
                    closeOnEsc: true,
                    closeOnOuterClick: true,
                }
            );
        }
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