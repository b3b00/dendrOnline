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

        public void AddChild(string name)
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
                        Child.Add(new LeafNote(name));
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
                        Child.Add(sub);
                    }
                    (sub as NodeNote)?.AddChild(name);

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
            builder.Append(tab).AppendLine(Name);
            foreach (var sub in Child)
            {
                builder.AppendLine(sub.Dump(tab + "  "));
            }

            return builder.ToString();
        }
    }
}