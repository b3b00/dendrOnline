<style>
    .draft  {
        color: red;
    }
    .normal  {
        color : black;
    }
</style>

<script lang="ts">

    import { onMount, getContext, setContext } from 'svelte';
    
    import {repository} from "../scripts/dendronStore";
    import {DendronClient} from "../scripts/dendronClient";
    import SvelteMarkdown from 'svelte-markdown'
    import {Note, TaggedNote} from '../scripts/types'
    import type { Context } from 'svelte-simple-modal';
    import ErrorDialog from './ErrorDialog.svelte';
    import CodeMarkdown from './CodeMarkdown.svelte';
    import type { ViewContext } from '../scripts/types';

    import 'highlight.js/styles/github-dark.css';
  import TaskRenderer from './TaskRenderer.svelte';
  
   

    const modal = getContext<Context>('simple-modal');
    
    export let id:string = "";

    let content:string = "";
    
    let title:string = "";
    
    let titleStyle:string = "normal"

    let note: Note|undefined = undefined;

    let backLinks: Note[];

    let getNoteId = function () : string {
        return id;
    }

    setContext<ViewContext>('view-context', {
        getNoteId : getNoteId,
    });

    
    
    onMount(async () => {
        console.log("get assets (name + url)");
    });
</script>
<h2>here will soon be an assets management.</h2>