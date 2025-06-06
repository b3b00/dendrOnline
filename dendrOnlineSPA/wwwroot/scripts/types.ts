import { TVNode } from "@bolduh/svelte-nested-accordion/src/AccordionTypes";

export class Node implements TVNode {
  name: string;
  isLeaf: boolean;
  isNode: boolean;
  deployed: boolean;
  selected: boolean;
  edited: boolean;
  children: Node[];
  id: string;
}

export class HierarchyAndSha {
  hierarchy: Node;
  sha: string;
}

export interface NoteHeader {
  id: string;
  name: string;
  title: string;
  description: string;
  lastUpdatedTS: number;
  createdTS: number;
}

export interface Note {
  header: NoteHeader;
  body: string;
  sha: string | undefined;
}

export interface TaggedNote {
  isDraft: boolean;
  note: Note;
}

export interface Repository {
  id: string;
  name: string;
  owner:string;
}

export interface SelectionItem {
  id: string;
  label: string;
}


export const emptyNote: Note = {
  header: {
    id: "**none**",
    name: "",
    title: "",
    description: "",
    lastUpdatedTS: 0,
    createdTS: 0,
  },
  body: "",
  sha: undefined,
};

export enum ConflictCode {
    NoConflict,
    Modified,
    Deleted,
    Created
}

export enum BackEndResultCode {
  Ok,
  Conflict,
  InternalError,
  NotFound
}



export enum ResultCode {
  Modified,
  Deleted,
  Created,
}


export interface BackEndResult<T> {
    theResult: T|undefined;
    code: BackEndResultCode;
    conflictCode : ConflictCode;
    isOk: boolean;
    errorMessage: string;
}

export class Dendron {
  hierarchy: Node;
  notes: Note[];
  repositoryId: string;
  repositoryName: string;
  repositoryOwner: string;
  isFavoriteRepository:boolean;  
}

export class ImageAsset {
  name: string;
  url: string;
}

export interface NoteFilter {
  filter : string,
  searchInNotes : boolean
}

export interface Favorite {
  user : number,
  repository : number,
  repositoryName : string
}

export interface ViewContext {
  getNoteId() : string;  
}