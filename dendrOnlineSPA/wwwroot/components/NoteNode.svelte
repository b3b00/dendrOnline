

<script lang="ts">
    import { onMount, getContext } from 'svelte';
    import {push} from 'svelte-spa-router'
    import {deleteNote, unDraft, unloadNote, repository, setTree, isDraft} from '../scripts/dendronStore.js';
    import Fa from 'svelte-fa/src/fa.svelte';
    import { faPlus, faTrashCan } from '@fortawesome/free-solid-svg-icons/index.js';
    import PromptDialog from "./PromptDialog.svelte";
    import ConfirmDialog from "./ConfirmDialog.svelte";
    import Modal from 'svelte-simple-modal';
    import {DendronClient} from "../scripts/dendronClient.js";
    const modal = getContext('simple-modal');
    import {Node} from '../scripts/types';

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
        console.log(`note creation canceled`);
    }
    
    const onCreationOk = async (noteId) => {
        console.log(`note creation requested : ${noteId}`);
        push(`/new/${noteId}`);
    }
    
    const onDeletionOkay = async (deleteChildren) => {
        console.log('do some note deletion work here '+nodeTitle);
        let recurse = false;
        if (deleteChildren === undefined || deleteChildren === null || deleteChildren === false) {
            recurse = false;
        }
        else {
            recurse = true;
        }
        deleteNote(data, recurse);
        unDraft(data.id);
        unloadNote(data.id);
        let newTree = await DendronClient.DeleteNote($repository.id,data.id,deleteChildren)
        setTree(newTree);
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
                option: 'Also delete children',
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
    <a href="#/edit/{data.name}" style="{isDraft(data.name) ? 'color:red': ''}">{nodeTitle}</a>
    <Modal>
        <span tabindex="-5" role="button" style="cursor: pointer" on:keydown={(e) => { e.preventDefault(); showCreationDialog(data);}} on:click={(e) => { e.preventDefault(); showCreationDialog(data);}}><Fa icon="{faPlus}" >PLUS</Fa></span>
        <span tabindex="-5" role="button" style="cursor: pointer" on:keydown={(e) => { e.preventDefault(); showDeletionDialog(data);}} on:click={(e) => { e.preventDefault(); showDeletionDialog(data);}}><Fa icon="{faTrashCan}" >TRASH</Fa></span>
    </Modal>
    
</a>