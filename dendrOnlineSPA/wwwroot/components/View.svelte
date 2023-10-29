<style>
    draft  {
        color: red;
    }
    normal  {
        color : black;
    }
</style>

<script>

    import { onMount } from 'svelte';
    import {
        getTitle, 
        setNoteId,
        isDraftNote,
        setLoadedNote,
        getNote,
    } from "../scripts/dendronStore.js";
    import {repository} from "../scripts/dendronStore.js";
    import {DendronClient} from "../scripts/dendronClient.js";
    import SvelteMarkdown from 'svelte-markdown'
    export let params = {}

    let id = "";

    let content = "";
    
    let title = "";
    
    let titleStyle = "normal"

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
    <SvelteMarkdown source={content}/>
    <br>
</div>