using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackEnd
{

    public class NodeNote : INoteHierarchy
    {
        public string Name { get; set; }

        public List<INoteHierarchy> Child { get; set; }

        public bool IsRoot => string.IsNullOrWhiteSpace(Name);

        public bool IsNode => true;

        public bool IsLeaf => false;

        public bool Deployed { get; set; } = false;
        
        public bool Selected { get; set; } = false;
        public INoteHierarchy GetSelectedNode()
        {
            if (Selected)
            {
                return this;
            }

            foreach (var node in Child)
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
            Child.ForEach(x => x.Deploy(currentNote));
        }

        public NodeNote(string name)
        {
            Name = name;
            Child = new List<INoteHierarchy>();
        }

        public void AddChild(string name, string currentNote)
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
                    if ((!Child.Any() || Child.All(x => x.Name != name)))
                    {
                        var leaf = new LeafNote(name);
                        leaf.Selected = name == currentNote;
                        Child.Add(leaf);
                    }
                }
                else
                {
                    var nextNodeIndex = subName.IndexOf(".");
                    subName = subName.Substring(0, nextNodeIndex);
                    var FqnSubName = (!IsRoot ? Name + "." : "") + subName;
                    var sub = Child.FirstOrDefault(x => x.Name == FqnSubName);
                    if (sub == null)
                    {
                        sub = new NodeNote(FqnSubName);
                        sub.Selected = FqnSubName == currentNote;
                        Child.Add(sub);
                    }
                    (sub as NodeNote)?.AddChild(name,currentNote);

                }
            }
        }

        public INoteHierarchy Filter(string filter)
        {

            var filteredChildren = Child.Select(x => x.Filter(filter)).Where(x => x != null);

            if (Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase) || filteredChildren.Any())
            {
                NodeNote node = new NodeNote(Name);
                node.Child.AddRange(filteredChildren);
                return node;
            }

            return null;
        }

        public string Dump(string tab)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(tab).Append(Name).AppendLine(Selected ? "*":"");
            foreach (var sub in Child)
            {
                builder.AppendLine(sub.Dump(tab + "  "));
            }

            return builder.ToString();
        }
    }
}