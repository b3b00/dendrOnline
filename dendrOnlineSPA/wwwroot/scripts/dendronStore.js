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

export function addNote(note) {
    // we will need something really brillant here
    const id = note.header.id;
    const path = id.split('.');
    const parentPath = path.slice(0, path.length - 1);
    tree.update(r => {
        let i = 0;
        let parent = r;
        while (i < parentPath.length && parent) {
            const currentItem = parentPath[i];
            const currentPath = parentPath.slice(0,i+1).join('.');
            let p = parent.children.filter(x => x.id === currentPath)[0];
            if (p) {
                parent = p;
            }
            else {
                parent = undefined;
                break;
            }
            i++;
        }
        if (parent) {
            if (!parent.children) {
                parent.children = [];
            }
            parent.children.push({
                id:note.header.id,
                name:note.header.name,
                children:[],
                deployed:true,
                edited:true,
                selected:true
            });
        }
        return r;
    });
    updateNote(id, note);

}

export function deleteNote(note) {
    // we will need something really brillant here
    const id = note.id;
    const path = id.split('.');
    const parentPath = path.slice(0, path.length - 1);
    tree.update(r => {
        let i = 0;
        let parent = r;
        while (i < parentPath.length && parent) {
            const currentItem = parentPath[i];
            const currentPath = parentPath.slice(0,i+1).join('.');
            let p = parent.children.filter(x => x.id === currentPath)[0];
            if (p) {
                parent = p;
            }
            else {
                parent = undefined;
                break;
            }
            i++;
        }
        if (parent) {
            if (!parent.children) {
                parent.children = [];
            }
            parent.children = parent.children.filter(x => x.id !== note.id);            
        }
        return r;
    });
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

export function unloadNote(id) {
    loadedNotes.update((r) => {
        delete r[id];
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

