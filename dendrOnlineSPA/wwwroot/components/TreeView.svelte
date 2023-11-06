<script>

    import {onMount} from "svelte";
    import TreeViewNode from "./TreeViewNode.svelte";

    export let root = {};

    
    export let nodeTemplate;

    export let childAccessor;
    
    export let filter = null;

    
    let search;
    
    let currentRoot;
    
    $:{        
        search = search;
        if (filter) {
            currentRoot = filter(root,search);
        }        
    }

    onMount(async () => {        
        currentRoot = root;
    })

</script>

<!-- TODO : add filter -->
{#if (currentRoot) }
<input type="text" bind:value={search}/>
<TreeViewNode node={currentRoot} nodeTemplate={nodeTemplate} childAccessor={childAccessor}/>
    {:else}
    <p>loading...</p>
    {/if}