using MongoDB.Bson;

namespace BackEnd
{
    public class Favorite
    {

        
        public Favorite()
        {
            
        }

        public Favorite(long userId, long repositoryId)
        {
            User = userId;
            Repository = repositoryId;
        }
        
        public ObjectId _id { get; set; }
        public long User { get; set; }
        public long Repository { get; set; }
        
        public string RepositoryName { get; set; }
    }
    
    
}