using System.Collections.Generic;

namespace BackEnd
{
    public class Dendron
    {
        public INoteHierarchy Hierarchy { get; set; }
        
        public IList<Note> Notes { get; set; }
        
        public long RepositoryId { get; set; }
        
        public string RepositoryName { get; set; }
        
        public string RepositoryOwner { get; set; }
        
        public bool IsFavoriteRepository { get; set; }

        public Dendron(INoteHierarchy hierarchy, IList<Note> notes, long repositoryId, string repositoryOwner, bool isFavoriteRepository)
        {
            Hierarchy = hierarchy;
            Notes = notes;
            RepositoryId = repositoryId;
            IsFavoriteRepository = isFavoriteRepository;
            RepositoryOwner = repositoryOwner;
        }

        public Dendron(INoteHierarchy hierarchy, long repositoryId, string repositoryOwner, bool isFavoriteRepository) : this(hierarchy, new List<Note>(),repositoryId, repositoryOwner, isFavoriteRepository)
        {
        }
        
        public Dendron(INoteHierarchy hierarchy, string repositoryOwner=null) : this(hierarchy, new List<Note>(),-1, repositoryOwner, false)
        {
        }

        public void Put(Note note)
        {
            Notes.Add(note);
        }
    }
}