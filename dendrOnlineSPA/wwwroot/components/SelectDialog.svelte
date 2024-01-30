<script lang="ts">
    import { getContext } from 'svelte';
    import { SelectionItem} from '../scripts/types'
    import {Context} from 'svelte-simple-modal'
    import {onMount} from 'svelte';


    export let message;
    export let hasForm = false;
    export let onCancel = () => {};
    export let onOkay = (SelectionItem) => {};
    export let items:SelectionItem[]  = [];
    let displayedItems:SelectionItem[] = [];



    const { close } = getContext<Context>('simple-modal');

    let selection;
    let onChange = (select) => {			
        selection = select;        
        displayedItems = items.filter(x => {
            const selected = search === null || search === undefined || search == '' || x.label.toLocaleLowerCase().includes(search.toLocaleLowerCase());
            return selected;
        });
    };

    let onNavigateSelect = function(e) {
        const i = displayedItems.indexOf(selection);
        if (i >= 0) {
            if (e.key == 'ArrowDown') {           
                let newIndex = i + 1;
                newIndex = newIndex < 0 || newIndex > displayedItems.length-1 ? i : newIndex;                    
                selection = displayedItems[newIndex];
            }            
            else if (e.key == 'ArrowUp') {
                let newIndex = i - 1;
                newIndex = newIndex < 0 || newIndex > displayedItems.length-1 ? i : newIndex;                    
                selection = displayedItems[newIndex];
            }
            else if(e.key == 'Enter') {
                _onOkay();
            }            
            displayedItems = items.filter(x => {
            const selected = search === null || search === undefined || search == '' || x.label.toLocaleLowerCase().includes(search.toLocaleLowerCase());
            return selected;
        });
        } 
    }

    function _onCancel() {
        onCancel();
        close();
    }

    function _onOkay() {
        onOkay(selection);
        close();
    }

    $: filterItems()

    let search:string = "";

    let filterItems = function() {        
        displayedItems = items.filter(x => {
            const selected = search === null || search === undefined || search == '' || x.label.toLocaleLowerCase().includes(search.toLocaleLowerCase());
            return selected;
        });
    }

    let getStyle = (item:SelectionItem):string => {
        const style =  (item && selection && item.id == selection.id) ? 'selected' : 'notSelected';        
        return style;
    }


    onMount(async() => {        
       displayedItems = items;        
    });


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

    .selected {
        border-radius: 5%;
        border-width: 1px;
        border-style: dashed;        
        background-color: lightblue;        
    }

    .notSelected {
        border: none;
    }

</style>

<h2>{message}</h2>

{#if hasForm}
<input type="text" bind:value={search} on:keyup={filterItems}>


<div style="display:flex;flex-direction:column">
    {#each displayedItems as item}
        <button  on:keydown={(e) => {onNavigateSelect(e);}} on:click={() => onChange(item)} class={getStyle(item)} >{item.label}</button>
    {/each}

</div>
{/if}

<div class="buttons">
    <button on:click={_onCancel}>
        Cancel
    </button>
    <button on:click={_onOkay}>
        Okay
    </button>
</div>