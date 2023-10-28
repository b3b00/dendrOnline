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
            currentRoot = filter(root);
            console.log('TreeView.reactive',currentRoot);
        }        
    }

    onMount(async () => {        
        currentRoot = root;
        console.log('TreeView.mount:',currentRoot);
    })

</script>

<!-- TODO : add filter -->
<input type="text" bind:value={search}/>
<TreeViewNode node={currentRoot} nodeTemplate={nodeTemplate} childAccessor={childAccessor}/>