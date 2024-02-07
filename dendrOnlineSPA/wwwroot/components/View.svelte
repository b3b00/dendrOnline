<style>
    .draft  {
        color: red;
    }
    .normal  {
        color : black;
    }
</style>

<script lang="ts">

    import { onMount, getContext } from 'svelte';
    import {location} from 'svelte-spa-router'
    import {
        getTitle,
        setNoteId,
        setLoadedNote,
        loadedNotes,
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

    const modal = getContext<Context>('simple-modal');
    
    export let id:string = "";

    let content:string = "";
    
    let title:string = "";
    
    let titleStyle:string = "normal"

    let note: Note|undefined = undefined;

    let backLinks: Note[];

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

    let preprocessLinks = function(markdown:string) {
        const regex = /\[\[(.*)\]\]/ig;
        const matches = markdown.matchAll(regex);
        let processed = markdown;
        for (let match of matches) {
            const name = match[1];
            const tag = match[0]
            var note = getNoteFromStore(match[1]);  
            var description = note.note.header.description;
            processed = processed.replaceAll(tag,`[${description}](#/view/${name})`);
        }

        return processed;
    }
    
    onMount(async () => {                
        setNoteId(id);        
        const commits = await DendronClient.getCommits($repository.id,id);
        console.log(commits);
        if (commits.isOk) {
            for (let i = 0; i < commits.theResult.length; i++) {
                const commit = commits.theResult[i];
                const atDate = DendronClient.GetNote($repository.id,id,commit.tree.sha);
                if ((await atDate).isOk) {
                    console.log(`${id} @ ${commit.sha}`,(await atDate).theResult);
                }
            }
        }

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
    <SvelteMarkdown source={content}/>
    </div>
    <div style="display:inline">
        <h2>Back links :</h2>
        <ul>
            {#if backLinks && backLinks.length > 0}
                {#each backLinks as link (link.header.title)}
                    <li><a href="#/view/{link.header.title}">{link.header.description}</a></li>
                {/each}
            {/if}

        </ul>
    </div>
</div>