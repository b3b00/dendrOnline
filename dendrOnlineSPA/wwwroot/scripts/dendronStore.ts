import { Writable, writable } from 'svelte/store';
import {Node, Note, Repository, emptyNode, emptyNote, empty} from '../scripts/types';

//region repositories
export const repository: Writable<Repository|undefined> = writable();

export function setRepository(repo:Repository) {
    repository.update(r => { return repo });
}


export const repositories: Writable<Repository[]> = writable([]);

export function setRepositories(repos: Repository[]) {
    repositories.update(r => { return repos  });
}

export function addNote(note: Note) {
    // TODO we will need something really brillant here
    const id = note.header.title;
    const path = id.split('.');
    const parentPath = path.slice(0, path.length - 1);
    tree.update(r => {
        let i = 0;
        let parent: Node|undefined = r;
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
                name:note.header.title,
                children:[],
                deployed:true,
                edited:true,
                selected:true,
                isNode:false,
                isLeaf:true
            });
        }
        return r;
    });
    updateNote(id, note);

}

export function deleteNote(note: Node, recurse:boolean) {
    // we will need something really brillant here
    const id = note.id;
    const path = id.split('.');
    const parentPath = path.slice(0, path.length - 1);
    tree.update(r => {
        let i = 0;
        let parent: Node|undefined = r;
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

export const noteId:Writable<string> = writable("");


export function getTitle(description: string) {
    if (description.startsWith("'")) {
        description = description.substring(1);
    }
    if (description.endsWith("'")) {
        description = description.substring(0,description.length-1);
    }
    return description;
}
export function setNoteId(id: string) {
    noteId.update((r:string) => { return id  });
}

export const draftNotes:Writable<Note[]> = writable([]);


export function updateNote(id:string,note:Note) {
    draftNotes.update((r) => {
        r = r.filter(x => x.header.title != id);
        r.push(note);        
        return r;
    });
}

export function unDraft(id:string) {
    draftNotes.update((r) => {
        r = r.filter(x => x.header.title != id);
        return r;
    } )
}

export function getDraftNote(id:string): Note|undefined {
    let note:Note|undefined = undefined;
    draftNotes.update((r) => {
        note =undefined;
        let f = r.filter(x => x.header.title == id);
        if (f.length > 0) {
            note = f[0];
        }
        return r;
    });
    return note;
}

export function isDraft(id:string) {
    console.log(`isDraft(${id}) ? `)
    let drafted = false;
    draftNotes.update((r) => {
        console.log(`comparing ${id} with ${r.length} draft`);
        if (r.length > 0) {
            console.log(r.map(x => x.header.title));
        }
        drafted = r.some(x => { 
            const d = x.header.title == id;
            console.log(`isDrafted(${id}) // ${x.header.title}? => ${d}`);
            return d;
        });
        console.log(`after comparisons : ${drafted}`);
        return r;
    } );
    console.log(`isDraft(${id}) == ${drafted}`);
    return drafted;
}


export const loadedNotes:Writable<Note[]> = writable([]);


export function setLoadedNote(id:string,note:Note) {
    loadedNotes.update((r) => {
        r = r.filter(x => x.header.title != id);
        r.push(note);        
        return r;
    });
}

export function unloadNote(id:string) {
    loadedNotes.update((r) => {
        r = r.filter(x => x.header.title != id);
        return r;
    } )
}

export function getLoadedNote(id:string): Note|undefined {
    let note:Note|undefined = undefined;
    loadedNotes.update((r) => {
        note =undefined;
        let f = r.filter(x => x.header.title == id);
        if (f.length > 0) {
            note = f[0];
        }        
        return r;
    });
    return note;
}

// endregion

// region tree
export const tree:Writable<Node|undefined> = writable();

export function setTree(currentTree: Node) {
    tree.update(r => {  return currentTree  });
}

//endregion

