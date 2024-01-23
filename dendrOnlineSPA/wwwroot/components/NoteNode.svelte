

<script>
    import { onMount } from 'svelte';
    
    export let data;

    let nodeTitle = "";

    onMount(async () => {
        if (data?.name !== undefined && data?.name !== null) {
            nodeTitle = data?.name;
            nodeTitle = nodeTitle.substring(nodeTitle.lastIndexOf('.')+1);
        }
        else {
            nodeTitle = data?.name;
        }
    })

</script>

<a href="#/edit/{data.name}">{nodeTitle}</a>