import {repository, setRepository} from "./dendronStore.js";

export const DendronClient = {

    GetRepositories : async () => {

        const res = await fetch('/repositories');
        if (res.status >= 200 && res.status <= 299) {
            let allRepositories = await res.json()
            return allRepositories
        } else {
            return [];
        }
    },
    
    GetTree: async (repositoryId) => {
        const res = await fetch(`/notes/${repositoryId}`);
        if (res.status >= 200 && res.status <= 299) {
            let tree = await res.json()
            console.log(tree);
            return tree;
        } else {
            return {};
        }
    },
    
    GetNote: async(repositoryId, noteId) => {
        const res = await fetch(`/note/${repositoryId}/${noteId}`);
        if (res.status >= 200 && res.status <= 299) {
            let note = await res.json()
            return note;
        } else {
            return {};
        }
    },
    
    CreateNote : async (repositoryId, noteId, noteContent) => {
        return {};
    },
    
    DeleteNote: async (repositoryId, noteId) => {
        return true;
    }
    
}
