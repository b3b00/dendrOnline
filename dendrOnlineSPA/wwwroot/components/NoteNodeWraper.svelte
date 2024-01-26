

<script>
    
    import Modal from 'svelte-simple-modal';
    import NoteNode from "./NoteNode.svelte";

    export let data;

</script>
<Modal>
    <NoteNode data="{data}"/>
</Modal>