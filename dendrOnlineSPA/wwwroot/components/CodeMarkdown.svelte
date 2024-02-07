<script lang="ts">
	import hljs from 'highlight.js';
	import {onMount} from 'svelte';

	export let text = '';
	export let lang = 'text';

	let highlightedText = "";

	onMount(() => {
		if (!lang ||lang.startsWith('plaintext')) {
			lang = "text";
		}
		highlightedText = hljs.highlight(text, { language: lang }).value;
	})

	
</script>

<pre class={`language-` + lang}>
	<code class="hljs">{@html highlightedText}</code>
</pre>