<script lang="ts">
    import Repositories from './components/Repositories.svelte'
    import Tree from './components/Tree.svelte'    
    import EditWrapper from './components/EditWrapper.svelte';
    import ViewWrapper from './components/ViewWrapper.svelte'
    import Home from './components/Home.svelte'
    import NotFound from "./components/NotFound.svelte";
    import {draftNotes, loadedNotes, noteId, repository, setTree, tree} from "./scripts/dendronStore";
    import Fa from 'svelte-fa/src/fa.svelte';
    import { faList, faPen, faEye, faFolderTree, faRefresh } from '@fortawesome/free-solid-svg-icons/index.js';
    
    import Router, { push } from 'svelte-spa-router'
  import { Modal } from 'svelte-simple-modal';
  import { DendronClient } from './scripts/dendronClient';

    const routes = {
        // Exact path
        '/': Home,

        // Using named parameters, with last being optional
        '/repositories': Repositories,

        // Wildcard parameter
        '/edit/:id': EditWrapper,
        '/view/:id': ViewWrapper,
        '/tree/:id/:refresh?': Tree,
        '/new/:id': EditWrapper,

        // Catch-all
        // This is optional, but if present it must be the last
        '*': NotFound,
    }

    const refresh= async () => {
        const dendron = await DendronClient.GetDendron($repository.id);
        if (dendron.isOk) {
            
            setTree(dendron.theResult.hierarchy);
            $loadedNotes = dendron.theResult.notes;
            $draftNotes = [];
            push(`/tree/${$repository.id}`);
        }
        else {
            console.log(`an error happened ${dendron.code}-${dendron.conflictCode} : ${dendron.errorMessage}`);
            push(`/repositories`);
        }
    }

</script>

<header>

    <a class="logo" href="#/">Dendr-Online</a>

    <input id="nav" type="checkbox">
    <label for="nav" id="burger"></label>

    <nav>
        <ul>
            <li><a href="#/repositories" ><Fa icon="{faList}"/><span style="margin-left: 5px">Repositories</span></a></li>
            {#if $repository && $repository.id}
                <li><a href="#/tree/{$repository.id}/refresh" on:click={refresh}><Fa icon="{faRefresh}"/><span style="margin-left: 5px">Refresh tree</span></a></li>
                <li><a href="#/tree/{$repository.id}" ><Fa icon="{faFolderTree}"/><span style="margin-left: 5px">Tree</span></a></li>
                {/if}
            {#if $noteId}
            <li>
                <ul>
                    <li><a href="#/edit/{$noteId}"><Fa icon="{faPen}"/><span style="margin-left: 5px">Edit</span></a></li>
                    <li><a href="#/view/{$noteId}"><Fa icon="{faEye}"/><span style="margin-left: 5px">View</span></a></li>
                </ul>
            </li>
            {/if}
        </ul>
    </nav>

</header>

<main>
    <Modal>
        <Router {routes}/>
    </Modal>
</main>