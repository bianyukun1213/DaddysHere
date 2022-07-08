using DaddysHere.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DaddysHere.Services
{
    public class SonsService
    {
        private readonly IMongoCollection<Son> _sonsCollection;
        public SonsService(IOptions<DaddysHereDatabaseSettings> daddysHereDatabaseSettings)
        {
            var mongoClient = new MongoClient(daddysHereDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(daddysHereDatabaseSettings.Value.DatabaseName);
            _sonsCollection = mongoDatabase.GetCollection<Son>(daddysHereDatabaseSettings.Value.SonsCollectionName);
        }
        //public async Task<List<Son>> GetAsync() =>
        //    await _sonsCollection.Find(_ => true).ToListAsync();
        //public async Task<Son?> GetAsync(string id) =>
        //    await _sonsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        //public async Task CreateAsync(Son newSon) =>
        //    await _sonsCollection.InsertOneAsync(newSon);
        //public async Task UpdateAsync(string id, Son updatedSon) =>
        //    await _sonsCollection.ReplaceOneAsync(x => x.Id == id, updatedSon);
        //public async Task RemoveAsync(string id) =>
        //    await _sonsCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<Son>> GetSonsAsync()
        {
            return await _sonsCollection.Find(_ => true).ToListAsync();
        }
        public async Task<List<Son>> GetSonsByNameAsync(string name)
        {
            return await _sonsCollection.Find(s => s.Name == name).ToListAsync();
        }
        public async Task<List<Son>> GetSonsByDaddyAsync(string daddyName)
        {
            return await _sonsCollection.Find(s => s.Daddy == daddyName).ToListAsync();
        }
        public async Task<Son?> GetSonByNameAndDaddyAsync(string name, string daddyName)
        {
            return await _sonsCollection.Find(s => s.Name == name && s.Daddy == daddyName).FirstOrDefaultAsync();
        }
        public async Task<Son?> GetSonByIdAsync(string id)
        {
            return await _sonsCollection.Find(s => s.Id == id).FirstOrDefaultAsync();
        }
        public async Task CreateSonAsync(Son newSon)
        {
            await _sonsCollection.InsertOneAsync(newSon);
        }
        public async Task UpdateSonByIdAsync(string id, Son updatedSon)
        {
            await _sonsCollection.ReplaceOneAsync(x => x.Id == id, updatedSon);
        }
        public async Task DeleteSonByIdAsync(string id)
        {
            await _sonsCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
