import NoteNode from "../components/NoteNode.svelte";
import { repository, setRepository } from "./dendronStore";
import { Note, Node, Result, Repository, emptyNode, emptyNote } from "./types";

const Ok = <T>(data: T): Result<T> => {
  return { result: data, ok: true, error: undefined };
};

const Error = <T>(error: string): Result<T> => {
  return { result: undefined, ok: false, error: error };
};

export const DendronClient = {
  GetRepositories: async (): Promise<Result<Repository[]>> => {
    try {
      const res = await fetch("/repositories");
      if (res.status >= 200 && res.status <= 299) {
        let allRepositories = await res.json();
        return Ok(allRepositories);
      } else {
        return Error(`erreur : ${res.status} - ${res.statusText}`);
      }
    } catch (e) {
      return Error(`erreur : ${e.message}`);
    }
  },

  GetTree: async (repositoryId): Promise<Result<Node>> => {
    try {
      const res = await fetch(`/notes/${repositoryId}`);
      if (res.status >= 200 && res.status <= 299) {
        let tree = await res.json();
        return Ok(tree);
      } else {
        return Error(`erreur : ${res.status} - ${res.statusText}`);
      }
    } catch (e) {
      return Error(`erreur : ${e.message}`);
    }
  },

  GetNote: async (
    repositoryId: string,
    noteId: string
  ): Promise<Result<Note>> => {
    try {
      const res = await fetch(`/note/${repositoryId}/${noteId}`);
      if (res.status >= 200 && res.status <= 299) {
        let note = await res.json();
        return Ok(note);
      } else {
        return Error(`erreur : ${res.status} - ${res.statusText}`);
      }
    } catch (e) {
      return Error(`erreur : ${e.message}`);
    }
  },

  // CreateNote : async (repositoryId, noteId, noteContent): Promise<Note> => {
  //     return emptyNote;
  // },

  DeleteNote: async (repositoryId, noteId, recurse): Promise<Result<Node>> => {
    try {
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
      if (res.status >= 200 && res.status <= 299) {
        let tree = await res.json();
        return Ok(tree);
      } else {
        return Error(`erreur : ${res.status} - ${res.statusText}`);
      }
    } catch (e) {
      return Error(`erreur : ${e.message}`);
    }
  },

  SaveNote: async (repositoryId: string, note: Note): Promise<Result<Node>> => {
    try {
      const res = await fetch(`/note/${repositoryId}/${note.header.title}`, {
        // withCredentials: true,
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(note), // body data type must match "Content-Type" header
      });
      if (res.status >= 200 && res.status <= 299) {
        let tree = await res.json();
        return Ok(tree);
      } else {
        return Error(`erreur : ${res.status} - ${res.statusText}`);
      }
    } catch (e) {
      return Error(`erreur : ${e.message}`);
    }
  },
};
