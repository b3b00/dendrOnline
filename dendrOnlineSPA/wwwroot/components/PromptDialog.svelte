<script>
    import { getContext } from 'svelte';
    export let message;
    export let hasForm = false;
    export let onCancel = () => {};
    export let onOkay = () => {};

    const { close } = getContext('simple-modal');

    export let parent;
    let onChange = () => {};

    function _onCancel() {
        onCancel();
        close();
    }

    function _onOkay() {
        onOkay(parent);
        close();
    }

    $: onChange(parent)
</script>

<style>
    h2 {
        font-size: 2rem;
        text-align: center;
    }

    input {
        width: 50%;
    }

    .buttons {
        display: flex;
        justify-content: space-between;
    }
</style>

<h2>{message}</h2>

{#if hasForm}
    <input
            type="text"
            bind:value={parent}
            on:keydown={e => e.key === "Enter" && _onOkay()} />
{/if}

<div class="buttons">
    <button on:click={_onCancel}>
        Cancel
    </button>
    <button on:click={_onOkay}>
        Okay
    </button>
</div>