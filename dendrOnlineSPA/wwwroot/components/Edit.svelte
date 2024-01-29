<style>
    .draft {
        color: red;
    }

    .normal {
        color: black;
    }

    textarea {
        position: relative;
        width: 100%;
        height: 100%;
        top: 0;
        resize: none;
        border: 2px dashed;
    }
</style>
<script lang="ts">

    import {onMount} from 'svelte';
    import {
        repository,
        updateNote,
        setNoteId,
        setLoadedNote,
        draftNotes,
        unDraft,
        addNote,
        setTree,
        getDraftNote,
        getLoadedNote


    } from "../scripts/dendronStore";
    
    import {DendronClient} from "../scripts/dendronClient";
    import Fa from 'svelte-fa/src/fa.svelte';
    import { faFloppyDisk, faUndo } from '@fortawesome/free-solid-svg-icons/index.js';
    import {location} from 'svelte-spa-router'
    import {Node, Note, Repository, TaggedNote} from '../scripts/types';
    
    

    export let params = {}

    let id = "";

    let isNewNote = false;
    
    let content = "";

    let note: Note|undefined = undefined;

    let titleStyle = "normal"

    let floppyVisibility = "display:none";

    let previousContent = "";

    
    
    let description = "";
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

    


    
    let save = async function() {
        console.log(`save ${id} ? `);
        let n = getNoteFromStore(id);
        if (n.isDraft) {
            // let clone = {...n.note}
            // clone.body = content;
            let newTree = await DendronClient.SaveNote($repository.id, n.note);
            setTree(newTree);
            unDraft(n.note.header.title);
            setLoadedNote(n.note.header.title,n.note);
            n = getNoteFromStore(n.note.header.title);
            note = n.note;
            description = note.header.description;            
            titleStyle = n.isDraft ? "draft" : "normal";            
            previousContent = note.body;
            content = note.body;
            floppyVisibility = n.isDraft ? "display:inline" : "display:none";
        }
    }


    let undo = async function() {
        unDraft(id);
        var oldNote = getLoadedNote(id)
        content = oldNote.body;
        description = oldNote.header.description;
        titleStyle = "normal";
        floppyVisibility = "display:none";        
    }

    let update = function () {
        if (note.body != content || note.header.description != description) {
            floppyVisibility = "display:inline";
            let clone = JSON.parse(JSON.stringify(note))
            clone.body = content;
            clone.header.description = description;
            previousContent = content;            
            updateNote(id, clone);
            //description = note.header.description;
            titleStyle = "draft";
        } else {
            description = note.header.description;            
            titleStyle = "normal";
            floppyVisibility = "display:none";
            unDraft(id);
        }
    }

    const onMountEdit = async (id) => {
        isNewNote = false;
        var n = getNoteFromStore(id);
        if (n) {
            note = n.note;
            description = note.header.description;
            titleStyle = n.isDraft ? "draft" : "normal";
            previousContent = note.body;
            content = note.body;
            floppyVisibility = n.isDraft ? "display:inline" : "display:none";
        } else {
            note = await DendronClient.GetNote($repository.id, id);
            previousContent = note.body;
            content = note.body;
            setLoadedNote(id, note);
            description = note.header.description;
            titleStyle = $draftNotes.hasOwnProperty(note.header.title) ? "draft" : "normal";
        }
    }
    
    const onMountNew = async (noteId) => {
        isNewNote = true;
        note = {
            header : {
                name: noteId,
                id: noteId,
                description: "new note",
                title: noteId,
                lastUpdatedTS: 0,
                createdTS: 0,
            },
            body : "# Write something really smart here.",            
        }
        addNote(note);
        //updateNote(noteId,note);
        
        description = note.header.description;
        titleStyle = "draft";
        id = noteId;
        previousContent = note.body;
        content = note.body;
        floppyVisibility = "display:inline" ;
    }
    
    onMount(async () => {
        console.log(`current page is at ${location}`);
        id = params.note
        setNoteId(id);
        if ($location.startsWith("/new")) {
            await onMountNew(id);
        }
        else {
            await onMountEdit(id);
        }
        
    });
</script>
<div>

    <span>
        <h1 contenteditable="true" style="display:inline" class="{titleStyle}" 
        bind:textContent={description} 
        on:input={update}>{description}</h1>
    </span>


    <!-- svelte-ignore a11y-no-noninteractive-element-to-interactive-role -->
    <h1 on:click={save} on:keydown={save} role="button" tabindex="-1" style="{floppyVisibility};cursor:pointer"><Fa icon="{faFloppyDisk}" ></Fa></h1>
    <h1 on:click={undo} on:keydown={undo} role="button" tabindex="-1" style="{floppyVisibility};cursor:pointer"><Fa icon="{faUndo}" ></Fa></h1>
    <br>
    <textarea bind:value={content} rows="200" on:keyup={update}></textarea> 
    <br>
</div>