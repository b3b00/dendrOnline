using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Constants = dendrOnlineSPA.Constants;

namespace BackEnd
{
    public class MongoService : IMongoService
    {
        public const string collectionName = "favorite";
        
        private string _connectionString;

        private IMongoDatabase _database;

        private IMongoCollection<Favorite> _collection;

        private MongoClient _mongoClient;

        public MongoService(IConfiguration configuration) : this(configuration[Constants.MongoConnectionStringParameter], configuration[Constants.MongoDbNameParameter])
        {
        }
        public MongoService(string connectionString, string dbName)
        {
            _connectionString = connectionString;
            _mongoClient = new MongoClient(connectionString);
            _database = _mongoClient.GetDatabase(dbName);
            _collection = _database.GetCollection<Favorite>(collectionName);
        }

        public async Task<Favorite> GetFavorite(long userId)
        {
            var filter = Builders<Favorite>.Filter
                .Eq(r => r.User, userId);
            var t =  _collection.Find<Favorite>(filter);
            var tt = t != null && t.Any() ? t.First() : null;
            return tt;
        }

        public async Task SaveFavorite(Favorite favorite)
        {
            var filter = Builders<Favorite>.Filter
                .Eq(f => f.User, favorite.User);
            var existingFavorite = await _collection.Find(filter).FirstOrDefaultAsync();
            if (existingFavorite != null)
            {
                var update = Builders<Favorite>.Update
                    .Set(f => f.Repository, favorite.Repository);
                await _collection.UpdateOneAsync(filter, update);
            }
            else
            {
                await _collection.InsertOneAsync(favorite);
            }
        }

        public async Task SaveFavorite(long userId, long repositoryId)
        {
            await SaveFavorite(new Favorite(userId, repositoryId));
        }
    }
}