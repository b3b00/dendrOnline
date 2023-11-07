<script>

    import {repositories, repository, tree, setTree} from '../scripts/dendronStore.js';
    import { onMount } from 'svelte';
    import NoteNode from "./NoteNode.svelte";
    import { DendronClient} from "../scripts/dendronClient.js";
    import TreeNode from "./TreeNode.svelte";
    import TreeView from "./TreeView.svelte";
    
    
    let currentRepository = {};
    
    let currentTree = {};
    
    let childAccessor = (x) => {
        if (x!= null && x !== undefined && x.child != undefined && x.child != null && Array.isArray(x.child)) {
            return x.child;
        }
        return [];
    }

    let nodefilter = (node, search) => {
        if (search === undefined || search === null || search == '') {
            return node;
        }
        var child = childAccessor(node);
        if (child.length > 0) {
            var filtered = child.filter(x => nodefilter(x, search)).filter(x => x!= null);
            if ( node.name.toLowerCase().includes(search.toLowerCase())) {
                return node;
            }
            else if (filtered.length > 0) {
                console.log(`accepting ${node.name.toLowerCase()} with ${filtered.length} child`,filtered);
                return {name:node.name,
                    id:node.id,
                    child:filtered
                };
            }
            console.log(`reject node ${node.name.toLowerCase()} // >${search.toLowerCase()}<`);
            return null;
        }
        else {
            if (node.name.toLowerCase().includes(search.toLowerCase())) {
                return node;
            }
            console.log(`reject leaf ${node.name.toLowerCase()} // >${search.toLowerCase()}<`);
            return null;
        }
    }

    $: {
        currentTree = currentTree;
    }

    onMount(async () => {


        currentRepository = $repository;
        currentTree = $tree;
        if (currentTree === null || currentTree === undefined || currentTree == {} || !currentTree.hasOwnProperty('name')) {
            currentTree = DendronClient.GetTree(currentRepository.id);
            setTree(currentTree);
        }
    });

</script>
<div>
    {#await currentTree}
        <p>...loading note tree...</p>
    {:then tree}
        <TreeView root={tree} childAccessor={childAccessor} nodeTemplate={NoteNode} filter={nodefilter}></TreeView>
    {:catch error}
        <p style="color: red">{error.message}</p>
    {/await}
</div>