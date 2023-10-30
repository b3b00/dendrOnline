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
    return description.substring(1,description.length-1);
}
export function setNoteId(id) {
    noteId.update(r => { return id  });
}

export const draftNotes = writable({});

export function updateNote(id,content) {
    draftNotes.update(r => {
        r[id] = content;
        return r;
    });
}

export function isDraftNote(id) {        
    return draftNotes.hasOwnProperty(id)
}

export function getNote(id) {
    if (isDraftNote(id)) {
        return {
            isDraft: true,
            note : draftNotes[id]
        }
    }
    else if (loadedNotes.hasOwnProperty(id)) {
        return {
            isDraft: true,
            note : draftNotes[id]
        }
    }
    return null;
}

export const loadedNotes = writable({});

export function setLoadedNote(id,content) {
    loadedNotes.update(r => {
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

