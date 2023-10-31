<style>
    .draft  {
        color: red;
    }
    .normal  {
        color : black;
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
<script>

    import { onMount } from 'svelte';
    import {
        getTitle,
        updateNote,
        setNoteId,
        setLoadedNote,
        loadedNotes,
        draftNotes, unDraft
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

    let previousContent = "";
    let getNoteFromStore = function(id) {
        console.log(`Edit.getNoteFromSvelte(${id})`)
        if ($draftNotes.hasOwnProperty(id)) {
            console.log(`Edit.getNoteFromSvelte(${id}) - found in draft notes`,$draftNotes[id]);
            return {
                isDraft: true,
                note : $draftNotes[id]
            }
        }
        else if (loadedNotes.hasOwnProperty(id)) {
            console.log(`Edit.getNoteFromSvelte(${id}) - found in loaded notes`,$loadedNotes[id]);
            return {
                isDraft: false,
                note : $loadedNotes[id]
            }
        }
        console.log(`Edit.getNoteFromSvelte(${id}) - not found `);
        return null;
    }


    let update = function() {
        console.log("update()");
        if (note.body != content) {
            console.log("update() : updating...");
            let clone = { ...note }
            clone.body = content;
            previousContent = content;
            updateNote(id, clone);
            title = getTitle(note.header.description) + " *";
            titleStyle = "draft";
        }
        else {
            title = getTitle(note.header.description) ;
            titleStyle = "normal";
            unDraft(id);
        }
    }
    
    onMount(async () => {
        id = params.note
        console.log(`Edit.onMount(${id})`);
        setNoteId(id);
        var n = getNoteFromStore(id);
        console.log(`Edit.onMount(${id}) note from store :: `,n);
        if (n) {
            console.log(`Edit.onMount(${id}) [1] : note found`);
            note = n.note;
            title = getTitle(note.header.description)+(n.isDraft ? " *" : "");
            console.log(`Edit.onMount(${id}) [1] title=${title}`);
            titleStyle = n.isDraft ? "draft" : "normal";
            console.log(`Edit.onMount(${id}) [1] style=${titleStyle}`);
            previousContent = note.body;
            content = note.body;
        }
        else {
            console.log(`Edit.onMount(${id}) [2] : note not found`);
            note = await DendronClient.GetNote($repository,id);
            console.log(`Edit.onMount(${id}) [2] : note from back ::`,note);
            previousContent = note.body;
            content = note.body;
            setLoadedNote(id,note);
            title = getTitle(note.header.description)+($draftNotes.hasOwnProperty(note.header.title) ? " *" : "");
            console.log(`Edit.onMount(${id}) [2] title=${title}`);
            titleStyle = $draftNotes.hasOwnProperty(note.header.title) ? "draft" : "normal";
            console.log(`Edit.onMount(${id}) [2] style=${titleStyle}`);
        }        
    });
</script>
<div>
    <h1 class="{titleStyle}">{title}</h1>
    <br>
    <textarea  bind:value={content} rows="200" on:keyup={update}></textarea>
    <br>
</div>