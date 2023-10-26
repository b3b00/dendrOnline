<script>

    import { onMount } from 'svelte';
    import {editedNotes, setNote, setNoteId} from "../scripts/dendronStore.js";
    import {repository} from "../scripts/dendronStore.js";
    import {DendronClient} from "../scripts/dendronClient.js";
    import SvelteMarkdown from 'svelte-markdown'
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
        if ($editedNotes[id]) {
            console.log(`note >${id}< found in store`);
            console.log($editedNotes[id]);
            console.log($editedNotes[id].body);
            note = $editedNotes[id];
            content = note.body;
        }
        else {
            console.log(`note >${id}< not found in store => requesting back`);
            note = await DendronClient.GetNote($repository,id);
            content = note.body;
            console.log(note);
            console.log(note.body);
            console.log(note);
            setNote(id,note);
        }

    });
</script>
<div>
    <a href="#/">home</a>
    <br>
    <a href="#/tree">tree</a>
    <br>
    <a href="#/edit/{id}">Edit</a>

    <br>
    <h1>{note.header.description}</h1>
    <br>
    <SvelteMarkdown source={content}/>
    <br>
</div>