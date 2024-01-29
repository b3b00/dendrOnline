<style>
    .draft  {
        color: red;
    }
    .normal  {
        color : black;
    }
</style>

<script lang="ts">

    import { onMount } from 'svelte';
    import {
        getTitle,
        setNoteId,
        setLoadedNote,
        loadedNotes, draftNotes,
    } from "../scripts/dendronStore";
    import {repository} from "../scripts/dendronStore";
    import {DendronClient} from "../scripts/dendronClient";
    import SvelteMarkdown from 'svelte-markdown'
    import {Note} from '../scripts/types'
    export let params = {}

    let id = "";

    let content = "";
    
    let title = "";
    
    let titleStyle = "normal"

    let note: Note|undefined = undefined;

    let getNoteFromStore = function(id) {
        console.log(`View.getNoteFromSvelte(${id})`)
        if ($draftNotes.hasOwnProperty(id)) {
            console.log(`View.getNoteFromSvelte(${id}) - found in draft notes`,$draftNotes[id]);
            return {
                isDraft: true,
                note : $draftNotes[id]
            }
        }
        else if (loadedNotes.hasOwnProperty(id)) {
            console.log(`View.getNoteFromSvelte(${id}) - found in loaded notes`,$loadedNotes[id]);
            return {
                isDraft: false,
                note : $loadedNotes[id]
            }
        }
        console.log(`View.getNoteFromSvelte(${id}) - not found `);
        return null;
    } 
    
    onMount(async () => {        
        id = params.note
        setNoteId(id);
        var n = getNoteFromStore(id);
      
        if (n) {
            console.log(`View.onMount(${id}) - found note`,n);
            note = n.note;
            title = getTitle(note.header.description)+(n.isDraft ? " *" : "");
            console.log(`View.onMount(${id}) - title=${title}`,n);
            titleStyle = n.isDraft ? "draft" : "normal";
            console.log(`View.onMount(${id}) - style=${titleStyle}`,n);
            content = note.body;
        }
        else {
            console.log(`View.onMount(${id}) [2] - not found`);
            note = await DendronClient.GetNote($repository.id,id);
            console.log(`View.onMount(${id}) [2] - found note`,n);
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