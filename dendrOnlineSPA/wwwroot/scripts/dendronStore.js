import { writable } from 'svelte/store';


export const repository = writable({
    id:undefined,
    name:undefined
});

export function setRepository(repo) {
    repository.update(r => { return repo });
}


export const repositories = writable([]);

export function setRepositories(repos) {
    repositories.update(r => { return repos  });
}

export const noteId = writable("");

export function setNoteId(id) {
    noteId.update(r => { return id  });
}

export const editedNotes = writable({});

export function updateNote(id,content) {
    editedNotes.update(r => {
        r[id] = content;
        return r;
    });
}

export const loadedNotes = writable({});

export function setLoadedNote(id,content) {
    loadedNotes.update(r => {
        r[id] = content;
        return r;
    });
}

export const tree = writable({});

export function setTree(currentTree) {
    tree.update(r => {  return currentTree  });
}


