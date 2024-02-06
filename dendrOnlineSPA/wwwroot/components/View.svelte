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
        getLoadedNote
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
        const processed = markdown.replaceAll(regex, "[$1](#/view/$1)");
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
        }
        else {
            const n = await DendronClient.GetNote($repository.id,id);
            if (n.isOk) {
                note = n.theResult;
                content = preprocessLinks(note.body);
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
<div>
    <h1 class="{titleStyle}">{title}</h1>
    <br>
    <SvelteMarkdown source={content}/>
    <br>
</div>