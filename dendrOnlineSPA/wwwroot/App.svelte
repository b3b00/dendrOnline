<script lang="ts">
    import Repositories from './components/Repositories.svelte'
    import Tree from './components/Tree.svelte'
    import Edit from './components/Edit.svelte'
    import View from './components/View.svelte'
    import Home from './components/Home.svelte'
    import NotFound from "./components/NotFound.svelte";
    import {noteId, repository} from "./scripts/dendronStore";
    import Fa from 'svelte-fa/src/fa.svelte';
    import { faList, faPen, faEye, faFolderTree } from '@fortawesome/free-solid-svg-icons/index.js';
    
    import Router from 'svelte-spa-router'
  import { Modal } from 'svelte-simple-modal';

    const routes = {
        // Exact path
        '/': Home,

        // Using named parameters, with last being optional
        '/repositories': Repositories,

        // Wildcard parameter
        '/edit/:note': Edit,
        '/view/:note': View,
        '/tree/:repository': Tree,
        '/new/:note': Edit,

        // Catch-all
        // This is optional, but if present it must be the last
        '*': NotFound,
    }

</script>

<header>

    <a class="logo" href="#/">Dendr-Online</a>

    <input id="nav" type="checkbox">
    <label for="nav" id="burger"></label>

    <nav>
        <ul>
            <li><a href="#/repositories" ><Fa icon="{faList}"></Fa><span style="margin-left: 5px">Repositories</span></a></li>
            {#if $repository && $repository.id}
                <li><a href="#/tree/{$repository.id}"><Fa icon="{faFolderTree}"></Fa><span style="margin-left: 5px">Tree</span></a></li>
                {/if}
            {#if $noteId}
                <li><a href="#/edit/{$noteId}"><Fa icon="{faPen}"></Fa><span style="margin-left: 5px">Edit</span></a></li>
                <li><a href="#/view/{$noteId}"><Fa icon="{faEye}"></Fa><span style="margin-left: 5px">View</span></a></li>
            {/if}
        </ul>
    </nav>

</header>

<main>
    <Modal>
        <Router {routes}/>
    </Modal>
</main>