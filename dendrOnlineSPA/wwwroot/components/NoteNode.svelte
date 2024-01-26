

<script>
    import { onMount, getContext } from 'svelte';
    import {push} from 'svelte-spa-router'
    import {deleteNote, unDraft, unloadNote} from '../scripts/dendronStore.js';
    import Fa from 'svelte-fa/src/fa.svelte';
    import { faPlus, faTrashCan } from '@fortawesome/free-solid-svg-icons/index.js';
    import PromptDialog from "./PromptDialog.svelte";
    import ConfirmDialog from "./ConfirmDialog.svelte";
    import Modal from 'svelte-simple-modal';
    const modal = getContext('simple-modal');

    export let data;

    let nodeTitle = "";

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
        console.log(`note creation canceled`);
    }
    
    const onCreationOk = (noteId) => {
        console.log(`note creation requested : ${noteId}`);
        push(`/new/${noteId}`);
        // TODO router push to /Edit/{noteId} ? should be enough ? => need some changes on Edit.svelte I guess 
    }
    
    const onDeletionOkay = () => {
        console.log('do some note deletion work here '+nodeTitle);
        deleteNote(data);
        unDraft(data.id);
        unloadNote(data.id);
        // TODO : call backend deletion ! (only if really needed ?);        
    }
    
    const onDeletionCancel = () => {
        console.log('finally I dont want to kill this note '+nodeTitle);
    }
    const showCreationDialog = (data) => {
        modal.open(
            PromptDialog,
            {
                message: "What is your favorite colour ?",
                parent: data.id,
                hasForm: true,
                onCancel:onCreationCancel,
                onOkay:onCreationOk
            },
            {
                closeButton: false,
                closeOnEsc: false,
                closeOnOuterClick: false,
            }
        );
    };

    const showDeletionDialog = (data) => {
        modal.open(
            ConfirmDialog,
            {
                message: `Are you sure to delete note ${nodeTitle} ?`,
                parent: data.id,
                hasForm: true,
                onCancel:onDeletionCancel,
                onOkay:onDeletionOkay
            },
            {
                closeButton: false,
                closeOnEsc: false,
                closeOnOuterClick: false,
            }
        );
    };
    
</script>

<a name="{nodeTitle}">
    <a href="#/edit/{data.name}">{nodeTitle}</a>
    <Modal>
        <span tabindex="-5" role="button" style="cursor: pointer" on:keydown={(e) => { e.preventDefault(); showCreationDialog(data);}} on:click={(e) => { e.preventDefault(); showCreationDialog(data);}}><Fa icon="{faPlus}" >PLUS</Fa></span>
        <span tabindex="-5" role="button" style="cursor: pointer" on:keydown={(e) => { e.preventDefault(); showDeletionDialog(data);}} on:click={(e) => { e.preventDefault(); showDeletionDialog(data);}}><Fa icon="{faTrashCan}" >TRASH</Fa></span>
    </Modal>
    
</a>