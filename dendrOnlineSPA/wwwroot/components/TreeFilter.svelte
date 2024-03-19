
<script lang="ts">
    import { onMount,createEventDispatcher } from 'svelte';
  import { NoteFilter } from '../scripts/types';
  import Switch from './Switch.svelte';

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

<div style="display:flex;flex-direction:column">
    <div style="display:flex;flex-direction:row">        
        <input type="text"  bind:value={filter} placeholder="search the notes ..."/>
    </div>
    <div style="display:flex;flex-direction:row">
        <label for="searchInNotes">Search in notes : </label>        
        <Switch id="searchInNotes" bind:checked={searchInNotes}/>            
    </div>    
    <div><button on:click={reset}>Reset</button></div>
</div>