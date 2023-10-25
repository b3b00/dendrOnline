<script>
	import Repositories from './components/Repositories.svelte'
	import Tree from './components/Tree.svelte'
	import Edit from './components/Edit.svelte'
	import View from './components/View.svelte'
	import Home from './components/Home.svelte'
	import NotFound from "./components/NotFound.svelte";
	
	import Router from 'svelte-spa-router'

	const routes = {
		// Exact path
		'/': Home,

		// Using named parameters, with last being optional
		'/repositories': Repositories,

		// Wildcard parameter
		'/edit/:note': Edit,
		'/view/:note' : View,
		'/tree/:repository' : Tree,

		// Catch-all
		// This is optional, but if present it must be the last
		'*': NotFound,
	}
	
</script>

<style>
	main {
		font-family: sans-serif;
		text-align: center;
	}
</style>

<main>
	
	<h1>Dendr-Online</h1>
	<Router {routes}/>
</main>