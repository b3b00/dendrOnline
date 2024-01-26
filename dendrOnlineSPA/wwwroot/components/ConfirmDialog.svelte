<script>
    import { getContext } from 'svelte';
    export let message;
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
        onOkay();
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
    <button on:click={_onCancel}>
        No
    </button>
    <button on:click={_onOkay}>
        Yes
    </button>
</div>