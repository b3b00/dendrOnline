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

        public string Dump(string tab)
        {
            return $"{tab}{Name}";
        }
    }
}