using System.Threading.Tasks;

namespace BackEnd
{
    public interface IMongoService
    {
        public Task<Favorite> GetFavorite(long userId);

        public Task SaveFavorite(Favorite favorite);
        
        public Task SaveFavorite(long userId, long repositoryId);
        
    }
 
}