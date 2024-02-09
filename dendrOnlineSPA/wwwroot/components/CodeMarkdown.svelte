<script lang="ts">
	import hljs from 'highlight.js';
	import {onMount} from 'svelte';
	import mermaid from 'mermaid';

	export let text = '';
	export let lang = 'text';

	let content = "";
	let isMermaid = false;

	let container;

	onMount(async () => {
		if (!lang ||lang.startsWith('plaintext')) {
			lang = "text";
		}
		if (lang == 'mermaid') {
			isMermaid = true;
			let c = await mermaid.render('todo',text);
			container.innerHTML=c.svg;
			content = c.svg
		}
		else {
			isMermaid = false;
			content = hljs.highlight(text, { language: lang }).value;
		}
	})

	
</script>


{#if isMermaid} 
<div>
	<span bind:this={container}>
</div>
{:else}
<pre class={`language-` + lang}>
	<code class="hljs">{@html content}</code>
</pre>
{/if}