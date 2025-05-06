namespace dendrOnlineSPA.Model;

public class GhRepository
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Owner { get; set; }

    public GhRepository(long id, string name, string owner)
    {
        Id = id;
        Name = name;
        Owner = owner;
    }


}