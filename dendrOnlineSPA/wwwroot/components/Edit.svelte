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
<script lang="ts">

    import {onMount, getContext} from 'svelte';
    import {
        repository,
        updateNote,
        setNoteId,
        setLoadedNote,
        draftNotes,
        unDraft,
        addNote,
        setTree,
        getDraftNote,
        getLoadedNote,
        unloadNote,
        deleteNote,
        tree
    } from "../scripts/dendronStore";
    
    import {DendronClient} from "../scripts/dendronClient";
    import Fa from 'svelte-fa/src/fa.svelte';
    import { faFloppyDisk, faUndo, faTrashCan } from '@fortawesome/free-solid-svg-icons/index.js';
    import {location, push} from 'svelte-spa-router'
    import {Node, Note, Repository, TaggedNote, SelectionItem} from '../scripts/types';
    import ConfirmDialog from './ConfirmDialog.svelte';
    import SelectDialog from './SelectDialog.svelte';
    import ErrorDialog from './ErrorDialog.svelte';
    import type { Context } from 'svelte-simple-modal';
    
    
  const modal = getContext<Context>('simple-modal');
    

    export let id = "";

    let isNewNote = false;
    
    let content = "";

    let note: Note|undefined = undefined;

    let titleStyle = "normal"

    let floppyVisibility = "display:none";

    let previousContent = "";

    let textInput = null;    
    
    let description = "";


    let getDescendance = function(node:Node): Node[] {
        let all:Node[] = [node];
        if (node.children && node.children.length > 0) {
            for (let index = 0; index < node.children.length; index++) {
                const element = node.children[index];
                const subs = getDescendance(element);
                all = all.concat(subs);            
            }
        }
        return all;
    }


    let getAllNotes = function()  {
        return getDescendance($tree);
    }

    let getNoteFromStore = function (id): TaggedNote {   
        
        var draft = getDraftNote(id);
        if (draft) {
            return {
                isDraft:true,
                note:draft
            }
        }

        var loaded = getLoadedNote(id);
        if (loaded) {
            return {
                isDraft:false,
                note:loaded
            }
        }
        return null;
    }

    


    
    let save = async function() {
        let n = getNoteFromStore(id);
        if (n.isDraft) {
            let newTree = await DendronClient.SaveNote($repository.id, n.note);
            if (newTree.isOk) {
                setTree(newTree.theResult.hierarchy);
                unDraft(n.note.header.title);
                n.note.sha = newTree.theResult.sha;
                setLoadedNote(n.note.header.title,n.note);
                n = getNoteFromStore(n.note.header.title);
                note = n.note;
                description = note.header.description;            
                titleStyle = n.isDraft ? "draft" : "normal";            
                previousContent = note.body;
                content = note.body;
                floppyVisibility = n.isDraft ? "display:inline" : "display:none";
                return;
            }
            else {
                modal.open(
                    ErrorDialog,
                    {
                        message: `An error occured: ${newTree.errorMessage} `
                    },
                    {
                        closeButton: true,
                        closeOnEsc: true,
                        closeOnOuterClick: true
                    }
                );
            }
        }
    }


    let undo = async function() {
        unDraft(id);
        var oldNote = getLoadedNote(id)
        if (oldNote) {
            content = oldNote.body;
            description = oldNote.header.description;
            titleStyle = "normal";
            floppyVisibility = "display:none";        
        }
    }

    const onDeletionOkay = async (deleteChildren) => {
        let recurse = false;
        if (deleteChildren === undefined || deleteChildren === null || deleteChildren === false) {
            recurse = false;
        }
        else {
            recurse = true;
        }
        deleteNote(id, recurse);
        unDraft(id);
        unloadNote(id);
        let newTree = await DendronClient.DeleteNote($repository.id,id,deleteChildren)
        if (newTree.isOk) {
            setTree(newTree.theResult);
            push(`/tree/${$repository.id}`);
        }
        else {
            modal.open(
            ErrorDialog,
            {
                message: `An error occured: ${newTree.errorMessage} `
            },
            {
                closeButton: true,
                closeOnEsc: true,
                closeOnOuterClick: true
            }
        );
        }
    }
    
    const onCancel = () => {
    }


    

    const onSelectOkay = function(item: SelectionItem) {
        // TODO insert text at the right position and give focus back to the textarea
        const range = window.getSelection();
			const { selectionStart: start, selectionEnd: end } = textInput;
			textInput.setRangeText(`${item.label}]]`);			
			const newPosition = end+item.label.length+2;
			textInput.setSelectionRange(newPosition,newPosition);
            content = textInput.value;
            update();
			textInput.focus();			
    }

    let insertLink = async function() {
        let items = getAllNotes().map(x => {return {id:x.name,label:x.name}});
        modal.open(SelectDialog,
            {
                message: `Choose a note to link to: `,
                items: items,                
                hasForm: true,
                onCancel:onCancel,
                onOkay:onSelectOkay
            },
            {
                closeButton: true,
                closeOnEsc: true,
                closeOnOuterClick: true
            });
    }

    let deleteThisNote = async function() {              
        modal.open(ConfirmDialog,
            {
                message: `Are you sure to delete note ${note.header.description} ?`,
                option: 'Also delete children',
                parent: note.header.name,
                hasForm: true,
                onCancel:onCancel,
                onOkay:onDeletionOkay
            },
            {            
                closeButton: true,
                closeOnEsc: true,
                closeOnOuterClick: true
            });
    }

    let checkIfLinkInsertionIsNeeded = function() {
        const { selectionStart: start, selectionEnd: end } = textInput;
        const lastTwoChars = content.substring(start-2,start);
        if (lastTwoChars == '[[') {
            insertLink();
        }
        else {
        }
    }

    let update = function () {
        checkIfLinkInsertionIsNeeded();
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

    const onMountEdit = async (id) => {
        isNewNote = false;
        setNoteId(id);
        var n = getNoteFromStore(id);
        if (n) {
            note = n.note;
            description = note.header.description;
            titleStyle = n.isDraft ? "draft" : "normal";
            previousContent = note.body;
            content = note.body;
            floppyVisibility = n.isDraft ? "display:inline" : "display:none";
        } else {
            const n = await DendronClient.GetNote($repository.id, id);
            if (n.isOk) {
                note = n.theResult;
                previousContent = note.body;
                content = note.body;
                setLoadedNote(id, note);
                description = note.header.description;
                titleStyle = $draftNotes.hasOwnProperty(note.header.title) ? "draft" : "normal";
            }
            else {
                modal.open(
                    ErrorDialog,
                    {
                        message: `An error occured: ${n.errorMessage} `                                            
                    },
                    {
                        closeButton: true,
                        closeOnEsc: true,
                        closeOnOuterClick: true
                    }
                );
            }
        }
    }
    
    const onMountNew = async (noteId) => {
        isNewNote = true;
        note = {
            header : {
                name: noteId,
                id: noteId,
                description: "new note",
                title: noteId,
                lastUpdatedTS: 0,
                createdTS: 0,
            },
            sha:null,
            body : "# Write something really smart here.",            
        }
        addNote(note);
        //updateNote(noteId,note);
        
        description = note.header.description;
        titleStyle = "draft";
        id = noteId;
        previousContent = note.body;
        content = note.body;
        floppyVisibility = "display:inline" ;
    }
    
    onMount(async () => {
        setNoteId(id);
        if ($location.startsWith("/new")) {
            await onMountNew(id);
        }
        else {
            await onMountEdit(id);
        }
        
    });
</script>
<div>

    <span>
        <h1 contenteditable="true" style="display:inline" class="{titleStyle}" 
        bind:textContent={description} 
        on:input={update}>{description}</h1>
    </span>

    <h1 on:click={deleteThisNote} on:keydown={deleteThisNote} role="button" tabindex="-1" style="display:inline;cursor:pointer"><Fa icon="{faTrashCan}" ></Fa></h1>
    <!-- svelte-ignore a11y-no-noninteractive-element-to-interactive-role -->
    <h1 on:click={save} on:keydown={save} role="button" tabindex="-1" style="{floppyVisibility};cursor:pointer"><Fa icon="{faFloppyDisk}" ></Fa></h1>
    <h1 on:click={undo} on:keydown={undo} role="button" tabindex="-1" style="{floppyVisibility};cursor:pointer"><Fa icon="{faUndo}" ></Fa></h1>
    <br>
    <textarea bind:this={textInput} bind:value={content} rows="200" on:keyup={update}></textarea> 
    <br>
</div>