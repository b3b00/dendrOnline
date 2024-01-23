namespace dendrOnlineSPA.Model;

public class GhRepository
{
    public long Id { get; set; }
    
    public string Name { get; set; }

    public GhRepository(long id, string name)
    {
        Id = id;
        Name = name;
    }


}