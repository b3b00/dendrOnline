using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackEnd
{

    public class LeafNote : INoteHierarchy
    {
        public LeafNote(string name)
        {
            Name = name;
        }

        [JsonIgnore]
        public INoteHierarchy NoteHierarchy { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("id")] public string Id => Name;

        [JsonIgnore]
        public bool IsNode => false;

        [JsonIgnore]
        public bool IsLeaf => true;

        public bool Deployed { get; set; } = true;

        public bool Selected { get; set; } 
        
        public bool Edited { get; set; } 
        
        [JsonPropertyName("children")]
        public List<INoteHierarchy> Children { get; set; }
        
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