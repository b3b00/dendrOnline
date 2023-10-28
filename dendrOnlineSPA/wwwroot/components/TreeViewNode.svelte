<style>
    .treemargin {
        margin-left: 25px;
    }
</style>
<script>

    import {onMount} from "svelte";

    export let node;
    
    export let nodeTemplate;
    
    export let childAccessor;
    
    let child;
    let isNode;
    
    
    $: {
        node = node;
        console.log('TreeViewNode.reactive > node :',node);
        child = childAccessor(node);
        console.log(`TreeViewNode.reactive > ${node.name} child :`,child);
        isNode = child && Array.isArray(child) && child.length > 0;
        console.log(`TreeViewNode.reactive > ${node.name} isNode ${node.name} : ${isNode}`)
    }
    onMount(async () => {
        console.log('TreeViewNode.mount > node :',node);
        child = childAccessor(node);
        console.log('TreeViewNode.mount > ${node.name} child :',child);
        isNode = child && Array.isArray(child) && child.length > 0;
        console.log(`TreeViewNode.mount > ${node.name} > isNode  : ${isNode}`)
    })
    
</script>

{#if isNode}
    <details class="treemargin" style="text-align: left">
        <summary >
            <svelte:component this={nodeTemplate} data={node}/>
        </summary>

        {#each node.child as subNode}
            <svelte:self node={subNode} nodeTemplate={nodeTemplate} childAccessor={childAccessor}/>
        {/each}

    </details>
{:else}
    <div class="treemargin">
        <svelte:component this={nodeTemplate} data={node}/>
    </div>
{/if}


