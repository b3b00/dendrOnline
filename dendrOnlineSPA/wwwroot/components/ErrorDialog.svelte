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
    code {
        color:red
    }

    .buttons {
        display: flex;
        justify-content: space-between;
        margin-top: 25px;
    }
</style>

<code >{@html message.replace('\n','<br>')}</code>



<div class="buttons">    
    <button on:click={_onOkay}>
        Yes
    </button>
</div>