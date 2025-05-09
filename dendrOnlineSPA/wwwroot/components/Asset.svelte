<script lang="ts">

    export let asset:ImageAsset;

    import { onMount, getContext, setContext } from 'svelte';
  import { ImageAsset } from '../scripts/types';

</script>

<div style="display:flex;flex-direction:row;flex-wrap:wrap;align-items: center;justify-content: center;">
    <img src={asset.url} alt={asset.name} />
    <h3>{asset.name}</h3>
</div>
