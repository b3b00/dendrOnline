using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace BackEnd
{

    public class NodeNote : INoteHierarchy
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")] public string Id => Name;

        [JsonPropertyName("children")]
        public List<INoteHierarchy> Children { get; set; }

        
        public bool IsRoot => string.IsNullOrWhiteSpace(Name);

        
        public bool IsNode => true;

        
        public bool IsLeaf => false;

        public bool Deployed { get; set; }
        
        public bool Selected { get; set; } = false;

        public bool Edited { get; set; } = false;
        public INoteHierarchy NoteHierarchy { get; set; }

        public INoteHierarchy GetSelectedNode()
        {
            if (Selected)
            {
                return this;
            }

            foreach (var node in Children)
            {
                var selection = node.GetSelectedNode();
                if (selection != null)
                {
                    return selection;
                }
            }

            return null;
        }

        public void Deploy(string currentNote)
        {
            Deployed = currentNote != null && currentNote.Contains(currentNote);
            Children.ForEach(x => x.Deploy(currentNote));
        }

        public NodeNote(string name)
        {
            Name = name;
            Children = new List<INoteHierarchy>();
        }

        public void AddChild(string name, string currentNote, List<string>? editedNotes)
        {

            var subName = name;
            if (!IsRoot)
            {
                subName = name.Replace(Name + ".", "");
            }
            if (!string.IsNullOrEmpty(subName))
            {
                if (!subName.Contains('.'))
                {
                    if ((!Children.Any() || Children.TrueForAll(x => x.Name != name)))
                    {
                        var leaf = new LeafNote(name);
                        leaf.Selected = name == currentNote;
                        leaf.Edited = editedNotes !=null && editedNotes.Contains(name);
                        Children.Add(leaf);
                    }
                }
                else
                {
                    var nextNodeIndex = subName.IndexOf(".");
                    subName = subName.Substring(0, nextNodeIndex);
                    var FqnSubName = (!IsRoot ? Name + "." : "") + subName;
                    var sub = Children.Find(x => x.Name == FqnSubName);
                    if (sub == null)
                    {
                        sub = new NodeNote(FqnSubName);
                        sub.Selected = FqnSubName == currentNote;
                        sub.Edited = editedNotes !=null && editedNotes.Contains(FqnSubName);
                        Children.Add(sub);
                    }
                    (sub as NodeNote)?.AddChild(name,currentNote,editedNotes);

                }
            }
        }

        public INoteHierarchy Filter(string filter)
        {

            var filteredChildren = Children.Select(x => x.Filter(filter)).Where(x => x != null);

            if (Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase) || filteredChildren.Any())
            {
                NodeNote node = new NodeNote(Name);
                node.Children.AddRange(filteredChildren);
                return node;
            }

            return null;
        }

        public string Dump(string tab)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(tab).Append(Name).AppendLine(Selected ? "*":"");
            foreach (var sub in Children)
            {
                builder.AppendLine(sub.Dump(tab + "  "));
            }

            return builder.ToString();
        }
    }
}