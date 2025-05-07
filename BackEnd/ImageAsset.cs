namespace BackEnd
{
    public class ImageAsset
    {
        public ImageAsset(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public string Name { get; set; }
        
        public string Url { get; set; }
    }
}