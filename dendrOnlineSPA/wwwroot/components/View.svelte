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
    import {
        getTitle,
        setNoteId,
        setLoadedNote,
        draftNotes,
        getDraftNote,
        getLoadedNote,
        getBackLinks
    } from "../scripts/dendronStore";
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

    let getNoteFromStore = function (id): TaggedNote {   
        
        var draft = getDraftNote(id);
        if (draft) {
            return {
                isDraft:true,
                note:draft
            }
        }

        var loaded = getLoadedNote(id);
        if (loaded) {
            return {
                isDraft:false,
                note:loaded
            }
        }
        return null;
    }

    let removeLeadingAndTrailingQuotes = (title: string):string =>  {
        if (title.startsWith('\'')) {
            title = title.substring(1);
        }
        if (title.endsWith('\'')) {
            title = title.substring(0,title.length-1);
        }
        return title;
    }

    let preprocessLinks = function(markdown:string) {
        const linkRegex = /\[\[(.*)\]\]/ig;
        const matches = markdown.matchAll(linkRegex);
        let processed = markdown;
        for (let match of matches) {
            const name = match[1];
            const tag = match[0]
            var note = getNoteFromStore(match[1]);  
            var description = removeLeadingAndTrailingQuotes(note.note.header.description);
            processed = processed.replaceAll(tag,`[${description}](#/view/${name})`);
        }

        const imgRegex = /!\[\]\((.*)\)/ig;
        const imgMatches = processed.matchAll(imgRegex);
        for (let match of imgMatches) {
            const name = match[1];
            const tag = match[0]

            var images = `![](https://raw.githubusercontent.com/${$repository.owner}/${$repository.name}/refs/heads/main/notes/${name})<br>`

            //processed = processed.replaceAll(tag,`![](https://raw.githubusercontent.com/${$repository.owner}/${$repository.name}/refs/heads/main/notes/${name})`);
            processed = processed.replaceAll(tag,images);
        }

        return processed;
    }
    
    onMount(async () => {                
        setNoteId(id);        
        var n = getNoteFromStore(id);
      
        if (n) {
            note = n.note;
            title = getTitle(note.header.description)+(n.isDraft ? " *" : "");
            titleStyle = n.isDraft ? "draft" : "normal";
            content = preprocessLinks(note.body);
            backLinks = getBackLinks(id);
        }
        else {
            const n = await DendronClient.GetNote($repository.id,id);
            if (n.isOk) {
                note = n.theResult;
                content = preprocessLinks(note.body);
                backLinks = getBackLinks(id);
                setLoadedNote(id,note);
                title = getTitle(note.header.description)+($draftNotes.hasOwnProperty(note.header.title) ? " *" : "");
                titleStyle = $draftNotes.hasOwnProperty(note.header.title) ? "draft" : "normal";
                setLoadedNote(id,note);
            }
            else {
                modal.open(
                    ErrorDialog,
                    {
                        message: `An error occured: ${n.errorMessage} `,                                                                        
                    },
                    {
                        closeButton: true,
                        closeOnEsc: true,
                        closeOnOuterClick: true
                    }
                );
            }
        }

    });
</script>
<div style="display:flex; flex-direction: row; flex-wrap: wrap;">
    <div style="display:inline">
    <h1 class="{titleStyle}">{title}</h1>
    <br>
    <SvelteMarkdown renderers={{ code: CodeMarkdown, listitem: TaskRenderer	}} source={content} />
    </div>
    {#if backLinks && backLinks.length > 0}
    <div style="display:inline">
        <h2>Back links :</h2>
        <ul>
            
                {#each backLinks as link (link.header.title)}
                    <li><a href="#/view/{link.header.title}">{link.header.description}</a></li>
                {/each}
            

        </ul>
    </div>
    {/if}
</div>