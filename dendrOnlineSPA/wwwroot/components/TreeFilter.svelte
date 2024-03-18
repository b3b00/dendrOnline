
<script lang="ts">
    import { onMount,createEventDispatcher } from 'svelte';
  import { NoteFilter } from '../scripts/types';

    const dispatch = createEventDispatcher<{ "filterChanged": NoteFilter }>();

    let filter : string;
    let searchInNotes : boolean = false;


    function change(){     
        dispatch('filterChanged',{filter,searchInNotes});	    
    }

    function reset() {
        filter = "";
        searchInNotes = false;
    }

    $:{
        dispatch('filterChanged',{filter,searchInNotes});
    }

</script>

<div style="display:flex;flex-direction:row">
    <div>
        <label for="filter">Nom :</label>
        <input type="text" id="filter" bind:value={filter} />
    </div>
    <div>
        <label for="searchInNotes">Search in notes : </label>
        <input type="checkbox" id="searchInNotes" bind:checked={searchInNotes}/>            
    </div>    
    <div><button on:click={reset}>Reset</button></div>
</div>