<script>
    import { getContext } from 'svelte';
    export let message;

    const { close } = getContext('simple-modal');
    let onChange = () => {};



    async function  _onOkay() {        
        close();
    }

    $: onChange()
</script>

<style>
    h2 {
        font-size: 2rem;
        text-align: center;
    }

    .buttons {
        display: flex;
        justify-content: space-between;
    }
</style>

<h2>{message}</h2>



<div class="buttons">    
    <button on:click={_onOkay}>
        Yes
    </button>
</div>