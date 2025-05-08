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
    import type { ViewContext, ImageAsset } from '../scripts/types';

    import 'highlight.js/styles/github-dark.css';
  import TaskRenderer from './TaskRenderer.svelte';
  import Asset from './Asset.svelte';
  
   

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

    let assets: ImageAsset[] = [];
    
    onMount(async () => {
        let assetsResult = await DendronClient.GetImages($repository.id);
if (assetsResult.isOk) {
            assets = assetsResult.theResult;
        }
        else {
            modal.open(
                ErrorDialog,
                {
                    message: `An error occured: ${assetsResult.errorMessage} `
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
<h2>here will soon be an assets management.</h2>
{#each assets as asset}
    <Asset {asset} />
{/each}