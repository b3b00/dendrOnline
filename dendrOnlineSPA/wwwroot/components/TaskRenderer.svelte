<script lang="ts">

import {onMount} from 'svelte';

export let text: string;

export let task: boolean;

export let checked: boolean;

let id: string;

let  hash = async (message: string) => {
    const data = new TextEncoder().encode(message);
    const hashBuffer = await crypto.subtle.digest('SHA-256', data)
    const hashArray = Array.from(new Uint8Array(hashBuffer))
    const hashHex = hashArray.map((b) => b.toString(16).padStart(2, '0')).join('')
    return hashHex
  }

onMount(async () => {
    id = await hash(text);
})

</script>

{#if task}
    <li>
        <span><input type="checkbox" checked={checked} disabled style="padding-right: 15px; display:inline"/><span><slot></slot></span></span>        
    </li>
{:else}
    <li><slot></slot></li>
{/if}
