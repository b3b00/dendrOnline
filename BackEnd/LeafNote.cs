using System;

namespace BackEnd
{

    public class LeafNote : INoteHierarchy
    {
        public LeafNote(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public bool IsNode => false;

        public bool IsLeaf => true;

        public bool Deployed { get; set; } = true;

        public bool Selected { get; set; } = false;
        
        public string Dump(string tab)
        {
            return $"{tab}{Name} {(Selected ? "*" : "")}";
        }

        public INoteHierarchy GetSelectedNode()
        {
            if (Selected)
            {
                return this;
            }
            return null;
        }

        public void Deploy(string currentNote)
        {
            
        }

        public INoteHierarchy Filter(string filter)
        {
            if (Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
            {
                return this;
            }
            return null;
        }
    }
}