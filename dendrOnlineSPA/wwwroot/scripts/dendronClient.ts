import {
  Note,
  Node,  
  Repository,
  BackEndResult,
  BackEndResultCode,
  ConflictCode,
  HierarchyAndSha,
} from "./types";

const ErrorResult = <T>(error: string, code: BackEndResultCode): BackEndResult<T> => {
  return {
    theResult: undefined,
    code: code,
    conflictCode: ConflictCode.NoConflict,
    errorMessage: error,
    isOk: false,
  };
};

export const DendronClient = {
  GetRepositories: async (): Promise<BackEndResult<Repository[]>> => {
    try {
      const res = await fetch("/repositories",{credentials: 'include'});
      let allRepositories = await res.json();
      return allRepositories;

    } catch (e) {
      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },

  GetTree: async (repositoryId): Promise<BackEndResult<Node>> => {
    try {
      const res = await fetch(`/notes/${repositoryId}`,{credentials: 'include'});
      if (res.status == 204) {
        return {
          theResult : undefined,
          isOk:false,
          code: BackEndResultCode.NotFound,
          conflictCode:ConflictCode.NoConflict,
          errorMessage:`dendron tree is empty`
        };
      }
      let tree = await res.json();
      return tree;
    } catch (e) {
      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },

  GetNote: async (
    repositoryId: string,
    noteId: string
  ): Promise<BackEndResult<Note>> => {
    try {
      const res = await fetch(`/note/${repositoryId}/${noteId}`,{credentials: 'include'});
      let note = await res.json();
      return note;
    } catch (e) {
      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },

  // CreateNote : async (repositoryId, noteId, noteContent): Promise<Note> => {
  //     return emptyNote;
  // },

  DeleteNote: async (
    repositoryId : string,
    noteId : string,
    recurse : boolean
  ): Promise<BackEndResult<Node>> => {
    try {
      const res = await fetch(
        `/note/${repositoryId}/${noteId}?recurse=${recurse}`,
        {
          credentials: 'include',
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      let tree = await res.json();
      return tree;
    } catch (e) {      
      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },

  SaveNote: async (
    repositoryId: string,
    note: Note
  ): Promise<BackEndResult<HierarchyAndSha>> => {
    try {
      const res = await fetch(`/note/${repositoryId}/${note.header.title}`, {
        credentials: 'include',
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(note), // body data type must match "Content-Type" header
      });

      let tree = await res.json();
      return tree;
    } catch (e) {
      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },
};
