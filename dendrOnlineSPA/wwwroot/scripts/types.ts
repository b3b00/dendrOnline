
import { TVNode  }from '@bolduh/svelte-treeview'

export class Node implements  TVNode {
    name: string;
    isLeaf : boolean;
    isNode : boolean;
    deployed: boolean;
    selected: boolean;
    edited: boolean;
    children: Node[];
    id: string
}

export interface NoteHeader {
    id: string;
    name: string;
    title: string;
    description: string;
    lastUpdatedTS: number,
    createdTS: number;
}

export interface Note {
    header: NoteHeader,
    body: string
}

export interface TaggedNote {
    isDraft: boolean;
    note: Note; 
}

export interface Repository {
    id: string;
    name: string;
}

export let empty ='**none**';

export let emptyNode: Node = {
    id:empty,
    name: '',
    children: [],
    isLeaf : false,
    isNode : false,
    deployed: false,
    selected: false,
    edited: false
}

export let emptyNote: Note = 
    {
        header: {  
            id: empty,
            name: '',
            title: '',
            description: '',
            lastUpdatedTS: 0,
            createdTS: 0
        },
        body: ''
    }