<script lang="ts">
    import {setRepository, repositories, setRepositories, repository, setTree, draftNotes, loadedNotes} from '../scripts/dendronStore.js';
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

    let currentRepository : Repository | undefined;

    $:{
        currentRepository = $repository;
        allRepositories = $repositories;
        filter = filter;
        filteredRepositories = allRepositories.filter(
            x => x.name.toLowerCase().includes(filter.toLowerCase())
        );
    }

    onMount(async () => {
        currentRepository = $repository;
        let allRepositories = await DendronClient.GetRepositories();
        if (allRepositories.isOk) {
            setRepositories(allRepositories.theResult);
            filteredRepositories = allRepositories.theResult;
        }
        else {
            modal.open(
                ErrorDialog,
                {
                    message: `An error occured: ${allRepositories.errorMessage} `
                },
                {
                    closeButton: true,
                    closeOnEsc: true,
                    closeOnOuterClick: true,
                }
            );
        }
    });

</script>
<style>
    .repository {
        cursor: pointer;
    }
    .repository:hover {
        background-color: lightgray;
    }
</style>

<div>
    <input type="text" bind:value={filter} placeholder="search ..."/>
    {#if filteredRepositories.length > 0}
        {#each filteredRepositories as repository}
            <div class="w3-display-container repository" aria-hidden="true" on:click={() => {                            
                        if (currentRepository && currentRepository.id != repository.id) {
                            setRepository(repository);
                            setTree(null);
                            $loadedNotes = [];
                            $draftNotes = [];
                            push(`#/tree/${repository.id}/refresh`);
                        }
                        else {
                            setRepository(repository);
                            push(`#/tree/${repository.id}`);
                        }
                    }}>
                {repository.name}
            </div>
        {/each}
    {/if}

</div>