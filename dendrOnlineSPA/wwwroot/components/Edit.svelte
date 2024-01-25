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
<script>

    import {onMount} from 'svelte';
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
    import Fa from 'svelte-fa/src/fa.svelte';
    import { faFloppyDisk } from '@fortawesome/free-solid-svg-icons/index.js';
    

    export let params = {}

    let id = "";

    let content = "";

    let note = {
        header: {
            id: "",
            description: "",
            title: ""
        },
        body: ""
    };

    let titleStyle = "normal"

    let floppyVisibility = "display:none";

    let previousContent = "";

    
    
    let description = "";
    let getNoteFromStore = function (id) {        
        if ($draftNotes.hasOwnProperty(id)) {            
            return {
                isDraft: true,
                note: $draftNotes[id]
            }
        } else if ($loadedNotes.hasOwnProperty(id)) {            
            return {
                isDraft: false,
                note: $loadedNotes[id]
            }
        }
        return null;
    }

    


    
    let save = async function() {
        console.log(`save ${id} ? `);
        let n = getNoteFromStore(id);
        if (n.isDraft) {
            let clone = {...n.note}
            clone.body = content;
            let newNote = await DendronClient.SaveNote(repository.id, n.note);
            unDraft(n.note.header.title);
            setLoadedNote(newNote.header.title,newNote);
            n = getNoteFromStore(newNote.header.title);
            note = n.note;
            description = note.header.description;            
            titleStyle = n.isDraft ? "draft" : "normal";            
            previousContent = note.body;
            content = note.body;
            floppyVisibility = n.isDraft ? "display:inline" : "display:none";
        }
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

    onMount(async () => {
        id = params.note
        setNoteId(id);
        var n = getNoteFromStore(id);
        if (n) {
            note = n.note;            
            description = note.header.description;
            titleStyle = n.isDraft ? "draft" : "normal";
            previousContent = note.body;
            content = note.body;
            floppyVisibility = n.isDraft ? "display:block" : "display:none";
        } else {
            note = await DendronClient.GetNote($repository, id);
            previousContent = note.body;
            content = note.body;
            setLoadedNote(id, note);
            description = note.header.description;
            titleStyle = $draftNotes.hasOwnProperty(note.header.title) ? "draft" : "normal";
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
    <br>
    <textarea bind:value={content} rows="200" on:keyup={update}></textarea> 
    <br>
</div>