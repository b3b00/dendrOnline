namespace BackEnd
{
    public class Result<T>
    {
        public T TheResult { get; set; }
        
        public bool Ok { get; set; }
        
        public string Error { get; set; }
    }
}