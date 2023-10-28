<script>

    import { onMount } from 'svelte';
    import {editedNotes, updateNote, setNoteId,loadedNotes, setLoadedNote} from "../scripts/dendronStore.js";
    import {repository} from "../scripts/dendronStore.js";
    import {DendronClient} from "../scripts/dendronClient.js";
    import View from "./View.svelte";
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
    
    onMount(async () => {
        // TODO : if note ! loaded => load it
        // else simply display it
        id = params.note
        setNoteId(id);
        if ($loadedNotes[id]) {
            note = $loadedNotes[id];
            content = note.body;
        }
        else {
            note = await DendronClient.GetNote($repository,id);
            content = note.body;
            console.log(note);
            setLoadedNote(id,note);            
        }
        
    });
</script>
<div>
    <a href="#/">home</a>
    <br>
    <a href="#/tree">tree</a>
    <br>
    <a href="#/view/{id}">View</a>

    <br>
    <h1>{note.header.description}</h1>
    <br>
    <textarea bind:value={content}></textarea>
    <br>
</div>