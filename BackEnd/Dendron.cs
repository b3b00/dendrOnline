using System.Collections.Generic;

namespace BackEnd
{
    public class Dendron
    {
        public INoteHierarchy Hierarchy { get; set; }
        
        public IList<Note> Notes { get; set; }

        public Dendron(INoteHierarchy hierarchy, IList<Note> notes)
        {
            Hierarchy = hierarchy;
            Notes = notes;
        }

        public Dendron(INoteHierarchy hierarchy) : this(hierarchy, new List<Note>())
        {
            
        }

        public void Put(Note note)
        {
            Notes.Add(note);
        }
    }
}