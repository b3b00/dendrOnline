<script lang="ts">

import { onMount, getContext, setContext } from 'svelte';
import {TaskToggler} from '../scripts/TaskToggler';
import type { ViewContext } from '../scripts/types';
import { getDraftNote, updateNote, repository, getLoadedNote, unDraft, setTree } from '../scripts/dendronStore';
import { DendronClient } from '../scripts/dendronClient';
  import ErrorDialog from './ErrorDialog.svelte';



export let text: string;

export let task: boolean;

export let checked: boolean;

let id: string;

const modal = getContext<Context>('simple-modal');

onMount(async () => {
    id = await TaskToggler.hashItem(text);
})


const context = getContext<ViewContext>('view-context');

let toggle = async () => {
    checked = !checked; 
    let noteId = context.getNoteId();
    var note  = getLoadedNote(noteId);
    if (note === undefined) {
        note = getDraftNote(noteId);
    }
    if (note) {
        let content = await TaskToggler.Toggle(text, id, note.body);
        note.body = note.body = content;
        updateNote(noteId, note);
        const newTree = await DendronClient.SaveNote($repository.id, note);
        if (newTree.isOk) {
            setTree(newTree.theResult.hierarchy);
            unDraft(noteId);
        }
        else {
            modal.show(ErrorDialog, { title: 'Error', message: newTree.errorMessage });
        }
        
    }
}




</script>

{#if task}
    <li>
        <span><input type="checkbox" on:change={toggle} checked={checked} style="padding-right: 15px; display:inline"/><span><slot></slot></span></span>
    </li>
{:else}
    <li><slot></slot></li>
{/if}
