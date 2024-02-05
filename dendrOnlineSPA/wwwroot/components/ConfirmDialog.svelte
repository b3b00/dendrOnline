<script>
    import { getContext } from 'svelte';
    export let message;
    
    export let option;
    
    let checked = false;
    
    export let onCancel = () => {};
    export let onOkay = () => {};

    const { close } = getContext('simple-modal');
    let onChange = () => {};

    async function _onCancel() {
        await onCancel();
        option = false;
        close();
    }

    async function  _onOkay() {
        await onOkay(checked);
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
{#if option}
    <div style="display: flex;flex-direction: row">
        <label for="opt">{option}</label>
        <input id="opt" type="checkbox" bind:checked={checked}>
    </div>
{/if}


<div class="buttons">
    <button on:click={_onCancel}>
        No
    </button>
    <button on:click={_onOkay}>
        Yes
    </button>
</div>