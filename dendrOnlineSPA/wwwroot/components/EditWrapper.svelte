<script>
    import Edit from './Edit.svelte';
      
    export let params


  </script>
  
  {#key params.id}
    <Edit id={params.id} /> 
  {/key}