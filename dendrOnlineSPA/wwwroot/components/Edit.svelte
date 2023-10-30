<style>
    .draft  {
        color: red;
    }
    .normal  {
        color : black;
    }
</style>
<script>

    import { onMount } from 'svelte';
    import {
        getTitle,
        updateNote,
        setNoteId,
        setLoadedNote,
        loadedNotes,
        draftNotes
    } from "../scripts/dendronStore.js";
    import {repository} from "../scripts/dendronStore.js";
    import {DendronClient} from "../scripts/dendronClient.js";
    import View from "./View.svelte";
    import {get} from "svelte/store";
    export let params = {}

    let id = "";
    
    let content = "";
    
    let note = {
        header:{
            id:"",
            description:"",
            title:""
        },
    body:""};

    let title = "";

    let titleStyle = "normal"


    let getNoteFromStore = function(id) {        
        if ($draftNotes.hasOwnProperty(id)) {
            return {
                isDraft: true,
                note : $draftNotes[id]
            }
        }
        else if (loadedNotes.hasOwnProperty(id)) {
            return {
                isDraft: false,
                note : $loadedNotes[id]
            }
        }
        return null;
    }


    let update = function() {
        note.body = content;
        updateNote(id,note);
        title = getTitle(note.header.description)+" *";
        titleStyle = "draft";
    }
    
    onMount(async () => {
        id = params.note
        setNoteId(id);
        var n = getNoteFromStore(id);
        if (n) {
            note = n.note;
            title = getTitle(note.header.description)+(n.draft ? " *" : "");
            titleStyle = n.draft ? "draft" : "normal";
            content = note.body;
        }
        else {
            note = await DendronClient.GetNote($repository,id);
            content = note.body;
            setLoadedNote(id,note);
            title = getTitle(note.header.description)+($draftNotes.hasOwnProperty(note.header.title) ? " *" : "");
            titleStyle = $draftNotes.hasOwnProperty(note.header.title) ? "draft" : "normal";
        }        
    });
</script>
<div>
    <h1 class="{titleStyle}">{title}</h1>
    <br>
    <textarea bind:value={content} on:keyup={update}></textarea>
    <br>
</div>