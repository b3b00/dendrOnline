<script>
    import Repositories from './components/Repositories.svelte'
    import Tree from './components/Tree.svelte'
    import Edit from './components/Edit.svelte'
    import View from './components/View.svelte'
    import Home from './components/Home.svelte'
    import NotFound from "./components/NotFound.svelte";
    import {noteId, repository} from "./scripts/dendronStore.js";

    import Router from 'svelte-spa-router'

    const routes = {
        // Exact path
        '/': Home,

        // Using named parameters, with last being optional
        '/repositories': Repositories,

        // Wildcard parameter
        '/edit/:note': Edit,
        '/view/:note': View,
        '/tree/:repository': Tree,

        // Catch-all
        // This is optional, but if present it must be the last
        '*': NotFound,
    }

    function onClickBurger() {
        burgerMenuClass = "burger " + (opened ? "active" : "");
        navBarMenuClass = "navbar  menu " + (opened ? "active" : "");
        if (opened) {
            //navBarMenuStyle = "maxHeight:"+
        }
    }

    let opened = false;

    let navBarMenuStyle = "";
    let navBarMenuClass = "navbar menu";
    let burgerMenuClass = "burger";

</script>

<header>

    <a class="logo" href="#/">Dendr-Online</a>

    <input id="nav" type="checkbox">
    <label for="nav" id="burger"></label>

    <nav>
        <ul>
            <li><a href="#/repositories"><span class="material-icons">Lists</span>Repositories</a></li>
            {#if $repository.id}
            <li><a href="#/tree/{$repository.id}"><span class="material-icons">Account_Tree</span>Tree</a></li>
                {/if}
            {#if $noteId}
                <li><a href="#/edit/{$noteId}"><span class="material-icons">Edit</span>Edit</a></li>
                <li><a href="#/view/{$noteId}"><span class="material-icons">Visibility</span>View</a></li>
            {/if}
        </ul>
    </nav>

</header>

<main>
    <Router {routes}/>
</main>