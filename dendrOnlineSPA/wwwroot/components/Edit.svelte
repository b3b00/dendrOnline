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
        getNote,
        isDraftNote
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
    
    let update = function() {        
        updateNote(id,content);
        title = getTitle(note.header.description)+" *";
        titleStyle = "draft";   
    }
    
    onMount(async () => {
        id = params.note
        setNoteId(id);
        var n = getNote(id);
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
            title = getTitle(note.header.description)+(isDraftNote(note.header.title) ? " *" : "");
            titleStyle = isDraftNote(note.header.title) ? "draft" : "normal";
        }        
    });
</script>
<div>
    <h1 class="{titleStyle}">{title}</h1>
    <br>
    <textarea bind:value={content} on:change={update}></textarea>
    <br>
</div>