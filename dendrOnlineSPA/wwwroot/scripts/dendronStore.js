import { writable } from 'svelte/store';

//region repositories
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



// endregion

// region notes

export const noteId = writable("");


export function getTitle(description) {
    if (description.startsWith("'")) {
        description = description.substring(1);
    }
    if (description.endsWith("'")) {
        description = description.substring(0,description.length-1);
    }
    return description;
}
export function setNoteId(id) {
    noteId.update((r) => { return id  });
}

export const draftNotes = writable({});

export function updateNote(id,content) {
    draftNotes.update((r) => {
        r[id] = content;
        return r;
    });
}

export function unDraft(id) {
    draftNotes.update((r) => {
        delete r[id];
        return r;
    } )
}


export const loadedNotes = writable({});

export function setLoadedNote(id,content) {
    loadedNotes.update((r) => {
        r[id] = content;
        return r;
    });
}


// endregion

// region tree
export const tree = writable({});

export function setTree(currentTree) {
    tree.update(r => {  return currentTree  });
}

//endregion

