<script>
    import View from './View.svelte';
  
    export let params
  
  </script>
  
  {#key params.id}
    <View id={params.id} /> 
  {/key}