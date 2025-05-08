import {
  Note,
  Node,  
  Repository,
  Dendron,
  BackEndResult,
  BackEndResultCode,
  ConflictCode,
  HierarchyAndSha,
  Favorite,
  ImageAsset,
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

  GetDendron : async (repositoryId): Promise<BackEndResult<Dendron>> => {
    try {
      const res = await fetch(`/dendron/${repositoryId}`,{credentials: 'include'});
      if (res.status == 204) {
        return {
          theResult : undefined,
          isOk:false,
          code: BackEndResultCode.NotFound,
          conflictCode:ConflictCode.NoConflict,
          errorMessage:`dendron tree is empty`
        };
      }
      let dendron = await res.json();
      return dendron;
    } catch (e) {
      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },

  GetImages : async(repositoryId : string): Promise<BackEndResult<ImageAsset[]>> => {
    try {
      const res = await fetch(`/images/${repositoryId}`,{credentials: 'include'});
      if (res.status == 204) {
        return {
          theResult : undefined,
          isOk:false,
          code: BackEndResultCode.NotFound,
          conflictCode:ConflictCode.NoConflict,
          errorMessage:`no images`
        };
      }      
      let images = await res.json();
      return images;
    } catch (e) {
      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },


  GetFavoriteRepository : async (): Promise<BackEndResult<Favorite>> => {
    try {
      const res = await fetch(`/favorite`,{credentials: 'include'});
      if (res.status == 404) {
        return {
          theResult : undefined,
          isOk:false,
          code: BackEndResultCode.NotFound,
          conflictCode:ConflictCode.NoConflict,
          errorMessage:`no favorite dendron`
        };
      }
      if (res.status == 204) {
        return {
          theResult : undefined,
          isOk:false,
          code: BackEndResultCode.NotFound,
          conflictCode:ConflictCode.NoConflict,
          errorMessage:`dendron tree is empty`
        };
      }
      let favorite = await res.json();
      return favorite;
    } catch (e) {
      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },


  GetFavoriteDendron : async (): Promise<BackEndResult<Dendron>> => {
    try {
      const res = await fetch(`/favorite/dendron/`,{credentials: 'include'});
      if (res.status == 404) {
        return {
          theResult : undefined,
          isOk:false,
          code: BackEndResultCode.NotFound,
          conflictCode:ConflictCode.NoConflict,
          errorMessage:`no favorite dendron`
        };
      }
      if (res.status == 204) {
        return {
          theResult : undefined,
          isOk:false,
          code: BackEndResultCode.NotFound,
          conflictCode:ConflictCode.NoConflict,
          errorMessage:`dendron tree is empty`
        };
      }
      let dendron = await res.json();
      return dendron;
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
      const url = `/note/${repositoryId}/${note.header.title}`
      console.log(`saveNote :: PUT url`);
      const res = await fetch(`/note/${repositoryId}/${note.header.title}`, {
        credentials: 'include',
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(note), // body data type must match "Content-Type" header
      });
      console.log(`PUT => ${res.status} - ${res.statusText}`);
      let tree = await res.json();
      console.log(`note ${note.header.title} saved, tree updated`);
      return tree;
    } catch (e) {
      console.log(`saveNote :: error :: ${e.message}`,e);
      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },

  setFavorite: async (repositoryId: string) => {
    try {
      const res = await fetch(`favorite/${repositoryId}`, {
        credentials: 'include',
        method:"POST"
      });
      console.log(res);
    }
    catch(e) {
      console.log(e);
    }
  },

  addImage : async (repositoryId: string, image: File, type: string): Promise<BackEndResult<{filename:string}>> => {
    try {
      const arrayBuffer = await image.arrayBuffer();
      const formData = new FormData();
      formData.append("image", image);
      const res = await fetch(`/image/${repositoryId}`, {
        credentials: 'include',
        method: "POST",
        body: formData
      });
      if (res.status == 200)  {
        const result = await res.json();
        return result;
      } else  {
        return {theResult:{filename:""},code:BackEndResultCode.InternalError,conflictCode:ConflictCode.NoConflict,isOk:true,errorMessage:res.statusText};
      }
    } catch (e) {

      return ErrorResult(`Error : ${e.message}`, BackEndResultCode.InternalError);
    }
  },
 
};
