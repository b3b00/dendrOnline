using System.Collections.Generic;

namespace BackEnd
{
    public class Dendron
    {
        public INoteHierarchy Hierarchy { get; set; }
        
        public IList<Note> Notes { get; set; }
        
        public long RepositoryId { get; set; }
        
        public string RepositoryName { get; set; }
        
        public bool IsFavoriteRepository { get; set; }

        public Dendron(INoteHierarchy hierarchy, IList<Note> notes, long repositoryId, bool isFavoriteRepository)
        {
            Hierarchy = hierarchy;
            Notes = notes;
            RepositoryId = repositoryId;
            IsFavoriteRepository = isFavoriteRepository;
        }

        public Dendron(INoteHierarchy hierarchy, long repositoryId, bool isFavoriteRepository) : this(hierarchy, new List<Note>(),repositoryId, isFavoriteRepository)
        {
            
        }
        
        public Dendron(INoteHierarchy hierarchy) : this(hierarchy, new List<Note>(),-1, false)
        {
            
        }

        public void Put(Note note)
        {
            Notes.Add(note);
        }
    }
}