using System;

namespace BackEnd
{

    public interface INoteHierarchy
    {
        string Name { get; set; }

        string ShortName => Name.Substring(Math.Max(0, Name.LastIndexOf('.'))).Replace(".", "");

        bool IsNode { get; }

        bool IsLeaf { get; }

        string Dump(string tab);
        
        bool Deployed { get; set; }
        
        bool Selected { get; set; }

        INoteHierarchy GetSelectedNode();

        void Deploy(string currentNote);

        public INoteHierarchy Filter(string filter);
    }
}