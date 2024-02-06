import {
  Note,
  Node,  
  Repository,
  BackEndResult,
  BackEndResultCode,
  ConflictCode,
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
      const res = await fetch("/repositories");
      let allRepositories = await res.json();
      return allRepositories;

    } catch (e) {
      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },

  GetTree: async (repositoryId): Promise<BackEndResult<Node>> => {
    try {
      const res = await fetch(`/notes/${repositoryId}`);
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
      const res = await fetch(`/note/${repositoryId}/${noteId}`);
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
      console.log(`dendronClinet.deleteNote(${repositoryId},${noteId},${recurse}) => fetch`);
      const res = await fetch(
        `/note/${repositoryId}/${noteId}?recurse=${recurse}`,
        {
          // withCredentials: true,
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      console.log(`fetch => ${res.status} - ${res.statusText}`);
      let tree = await res.json();
      console.log(`client will return `,tree);
      return tree;
    } catch (e) {
      console.log(`exception on delete ${e.message}`)
      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },

  SaveNote: async (
    repositoryId: string,
    note: Note
  ): Promise<BackEndResult<Node>> => {
    try {
      const res = await fetch(`/note/${repositoryId}/${note.header.title}`, {
        // withCredentials: true,
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
