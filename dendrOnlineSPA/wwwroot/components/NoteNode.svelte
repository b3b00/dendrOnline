

<script lang="ts">
    import { onMount, getContext } from 'svelte';
    import ErrorDialog from './ErrorDialog.svelte';
    import {push} from 'svelte-spa-router'
    import {deleteNote, unDraft, unloadNote, repository, setTree, isDraft} from '../scripts/dendronStore.js';
    import Fa from 'svelte-fa/src/fa.svelte';
    import { faPlus, faTrashCan } from '@fortawesome/free-solid-svg-icons/index.js';
    import PromptDialog from "./PromptDialog.svelte";
    import ConfirmDialog from "./ConfirmDialog.svelte";    
    import {DendronClient} from "../scripts/dendronClient.js";
    import type { Context } from 'svelte-simple-modal';
    
    import {Node} from '../scripts/types';

    const modal = getContext<Context>('simple-modal');

    export let data: Node;

    let nodeTitle: string = "";

    onMount(async () => {
        if (data?.name !== undefined && data?.name !== null) {
            nodeTitle = data?.name;
            nodeTitle = nodeTitle.substring(nodeTitle.lastIndexOf('.')+1);
        }
        else {
            nodeTitle = data?.name;
        }     
    })

    const onCreationCancel = (noteId) => {
    }
    
    const onCreationOk = async (noteId) => {
        push(`/new/${noteId}`);
    }
    
    const onDeletionOkay = async (deleteChildren) => {
        console.log(`onDeletionOkay(${deleteChildren})`,$repository,data);
        let recurse = false;
        if (deleteChildren === undefined || deleteChildren === null || deleteChildren === false) {
            recurse = false;
        }
        else {
            recurse = true;
        }
        deleteNote(data.id, recurse);
        unDraft(data.id);
        unloadNote(data.id);
        console.log(`call client.deleteNote(${$repository.id},${data.id},${recurse})`);
        let newTree = await DendronClient.DeleteNote($repository.id,data.id,recurse)
        console.log('client returned',newTree);
        if (newTree.isOk) {
            console.log('setting new tree');
            setTree(newTree.theResult);
        }
        else {
            console.log(`opening error dialog with ${newTree.errorMessage}`);
            modal.open(
            ErrorDialog,
            {
                message: `An error occured: ${newTree.errorMessage} `
            },
            {
                closeButton: true,
                closeOnEsc: true,
                closeOnOuterClick: true
            })
        }        
    }
    
    const onDeletionCancel = () => {
        console.log('deletion has been canceled');
    }
    const showCreationDialog = (data) => {
        modal.open(
            PromptDialog,
            {
                message: "new note name : ",
                parent: data.id,
                hasForm: true,
                onCancel:onCreationCancel,
                onOkay:onCreationOk
            },
            {
                closeButton: true,
                closeOnEsc: true,
                closeOnOuterClick: true,
            }
        );
    };

    const showDeletionDialog = (data) => {
        modal.open(
            ConfirmDialog,
            {
                message: `Are you sure to delete note ${nodeTitle} ?`,
                option: 'Also delete children',
                parent: data.id,
                hasForm: true,
                oncancel: onDeletionCancel,
                onOkay: onDeletionOkay
            },
            {
                closeButton: true,
                closeOnEsc: true,
                closeOnOuterClick: true,
            }
        );
    };
    
</script>

<a name="{nodeTitle}">
    <a href="#/view/{data.name}" style="{isDraft(data.name) ? 'color:red': ''}">{nodeTitle}</a>
    
    <span tabindex="-5" role="button" style="cursor: pointer" on:keydown={(e) => { e.preventDefault(); showCreationDialog(data);}} on:click={(e) => { e.preventDefault(); showCreationDialog(data);}}><Fa icon="{faPlus}" /></span>
    <span tabindex="-5" role="button" style="cursor: pointer" on:keydown={(e) => { e.preventDefault(); showDeletionDialog(data);}} on:click={(e) => { e.preventDefault(); showDeletionDialog(data);}}><Fa icon="{faTrashCan}" /></span>
    
    
</a>