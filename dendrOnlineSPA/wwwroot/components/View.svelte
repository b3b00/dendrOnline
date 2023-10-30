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
        setNoteId,
        setLoadedNote,
        loadedNotes, draftNotes,
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
    
    onMount(async () => {        
        id = params.note
        setNoteId(id);
        var n = getNoteFromStore(id);
      
        if (n) {
            note = n.note;
            title = getTitle(note.header.description)+(n.isDraft ? " *" : "");
            titleStyle = n.isDraft ? "draft" : "normal";
            content = note.body;
        }
        else {
            
            note = await DendronClient.GetNote($repository,id);
            content = note.body;
            setLoadedNote(id,note);
            title = getTitle(note.header.description)+($draftNotes.hasOwnProperty(note.header.title) ? " *" : "");
            titleStyle = $draftNotes.hasOwnProperty(note.header.title) ? "draft" : "normal";
            setLoadedNote(id,note);
        }

    });
</script>
<div>
    <h1 class="{titleStyle}">{title}</h1>
    <br>
    <SvelteMarkdown source={content}/>
    <br>
</div>