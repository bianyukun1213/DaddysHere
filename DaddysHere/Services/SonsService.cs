using DaddysHere.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DaddysHere.Services
{
    public class SonsService
    {
        private readonly IMongoCollection<Son> _sonsCollection;
        private readonly ILogger<SonsService> _logger;
        public SonsService(IOptions<DaddysHereDatabaseSettings> daddysHereDatabaseSettings, ILogger<SonsService> logger)
        {
            var mongoClient = new MongoClient(daddysHereDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(daddysHereDatabaseSettings.Value.DatabaseName);
            _sonsCollection = mongoDatabase.GetCollection<Son>(daddysHereDatabaseSettings.Value.SonsCollectionName);
            _logger = logger;
            _logger.LogDebug(1, "NLog 已注入到 SonsService。");
        }
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
        public bool IsSonValid(Son son)
        {
            int mkLen = son.Markdown?.Length ?? 0;
            int TempLen = son.Template?.Length ?? 0;
            bool sonValid = son is not null && son.Name is not null && son.Daddy is not null && son.Name.Length <= 8 && son.Daddy.Length <= 8 && mkLen <= 400 && TempLen <= 12;
            return sonValid;
        }
        public void DeleteExpiredSons()
        {
            _logger.LogInformation("正在删除过期儿子。");
            var res = _sonsCollection.DeleteMany(x => x.Expiration <= DateTime.Now.Date && !x.Reserved);
            _logger.LogInformation("删除完成，共删除 {count} 个过期儿子。", res.DeletedCount);
        }
    }
}
