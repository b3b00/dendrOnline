<style>
    .treemargin {
        margin-left: 25px;
    }
</style>
<script>
    import {push} from 'svelte-spa-router'
    import {setNoteId} from '../scripts/dendronStore.js';
    export let node = {};
</script>

    {#if node.isNode}
        <details class="treemargin" style="text-align: left">
    <summary ><a href="#/edit/{node.name}">{node.name}</a></summary>

        {#each node.child as subNode}
            <svelte:self node={subNode}/>
        {/each}

        </details>
    {:else}
        <div><a href="#/edit/{node.name}">{node.name}</a></div>
    {/if}


