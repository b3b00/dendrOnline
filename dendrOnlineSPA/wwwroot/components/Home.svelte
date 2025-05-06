<script lang="ts">
    import {repository, isFavoriteRepository, tree, setTree, loadedNotes, getLoadedNote} from '../scripts/dendronStore.js';
    import { onMount, getContext } from 'svelte';    
    import { DendronClient} from "../scripts/dendronClient.js";       
    import {BackEndResult, Dendron, Node, NoteFilter, Repository} from '../scripts/types'
    import { push } from 'svelte-spa-router';
    import { Wave } from 'svelte-loading-spinners';
    
    let loading = false;
    let repositoryName = "";
    let hasFavorite = false;

    onMount(async () => {        
        
        let favorite = await DendronClient.GetFavoriteRepository();
        if (favorite.isOk) {
            let dendron:BackEndResult<Dendron> = undefined;
            console.log('found a favorite repo => will load it as dendron ',favorite);
            console.log('loading favorite tree from BackEnd')
            loading = true;
            hasFavorite = true;
            repositoryName = favorite.theResult.repositoryName; 
            dendron = await DendronClient.GetFavoriteDendron();

            console.log(dendron);

            if (dendron.isOk) {
                console.log(`favorite repository loaded : ${dendron.theResult.repositoryName}`);
                $repository = {id:dendron.theResult.repositoryId,name:dendron.theResult.repositoryName, owner:dendron.theResult.repositoryOwner};
                setTree(dendron.theResult.hierarchy);
                $isFavoriteRepository = true;
                push(`#/tree/${dendron.theResult.repositoryId}`);
            }
        }
        else {
            loading = false;
            hasFavorite = false;
        }
    });

</script>
<div>
    {#if loading && hasFavorite}
    <div class="spinner-item" title="Wave">
        <Wave  size="45" />
        <div class="spinner-title">Favorite <b>{repositoryName}</b> Dendron is loading...</div>
    </div>
    {:else}
    <h1>Dendr-Online</h1>
    <p>select a repository...</p>    
    {/if}
</div>