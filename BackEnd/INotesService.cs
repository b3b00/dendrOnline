using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BackEnd
{

    public interface INotesService
    {
        Task<Result<(string content, string sha)>> GetContent(string name);

        Task<Result<Note>> SetContent(string noteName, Note note);

        Task<Result<List<string>>> GetNotes();

        Task<Result<Note>> GetNote(string noteName);
        
        Task<Result<Note>> DeleteNote(string noteName);

        Task<Result<INoteHierarchy>> GetHierarchy(List<string> notes, string filter, string currentNote, List<string> editedNotes);

        void SetRepository(string name, long id);

        void SetUser(string name, long id);
        
        void SetAccessToken(string token);

        Task<Result<Dendron>> GetDendron();

        Task AddImage(IFormFile file, string fileName);

    }
}