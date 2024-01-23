using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackEnd
{

    [JsonDerivedType(typeof(NodeNote))]
    [JsonDerivedType(typeof(LeafNote))]
    public interface INoteHierarchy
    {
        string Name { get; set; }

        string ShortName => Name.Substring(Math.Max(0, Name.LastIndexOf('.'))).Replace(".", "");

        bool IsNode { get; }

        bool IsLeaf { get; }

        string Dump(string tab);
        
        bool Deployed { get; set; }
        
        bool Selected { get; set; }
        
        bool Edited { get; set; }
        INoteHierarchy NoteHierarchy { get; set; }
        

        public List<INoteHierarchy> Children { get; set; }

        INoteHierarchy GetSelectedNode();

        void Deploy(string currentNote);

        public INoteHierarchy Filter(string filter);
    }
}