<style>
    .treemargin {
        margin-left: 25px;
    }
</style>
<script>
    export let node = {};
</script>

    {#if node.isNode}
        <details class="treemargin" style="text-align: left">
    <summary>{node.name}></summary>

        {#each node.child as subNode}
            <svelte:self node={subNode}/>
        {/each}

        </details>
    {:else}
        <div class="treemargin">{node.name}</div>
    {/if}


