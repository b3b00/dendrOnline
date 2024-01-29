

<script lang="ts">
    
    import Modal from 'svelte-simple-modal';
    import NoteNode from "./NoteNode.svelte";
    import {Node} from '../scripts/types';

    export let data: Node;

</script>
<Modal>
    <NoteNode data="{data}"/>
</Modal>