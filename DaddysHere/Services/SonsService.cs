using DaddysHere.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.RegularExpressions;

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
            return await _sonsCollection.Find(s => s.Name.ToLower() == name.ToLower()).ToListAsync();
        }
        public async Task<List<Son>> GetSonsByDaddyAsync(string daddyName)
        {
            return await _sonsCollection.Find(s => s.Daddy.ToLower() == daddyName.ToLower()).ToListAsync();
        }
        public async Task<Son?> GetSonByNameAndDaddyAsync(string name, string daddyName)
        {
            return await _sonsCollection.Find(s => s.Name.ToLower() == name.ToLower() && s.Daddy.ToLower() == daddyName.ToLower()).FirstOrDefaultAsync();
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
        public async Task<bool> DoesSonsCountReachLimitValueAsync()
        {
            const long LIMIT_VALUE = 10000;
            long estimatedCount = await _sonsCollection.EstimatedDocumentCountAsync();
            return estimatedCount >= LIMIT_VALUE;
        }
        public bool IsSonValid(Son son)
        {
            bool mkValid = (son.Markdown?.Length ?? 0) <= 400;
            bool tempValid = (son.Template?.Length ?? 0) <= 12;
            bool avatarValid = true;
            bool daddyAvatarValid = true;
            string picRegexStr = @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)+\.(jpg|png|webp|gif)";
            if (!string.IsNullOrEmpty(son.Avatar))
            {
                var m = Regex.Matches(son.Avatar, picRegexStr);
                if (m.Count != 1)
                {
                    avatarValid = false;
                }
            }
            if (!string.IsNullOrEmpty(son.DaddyAvatar))
            {
                var m = Regex.Matches(son.DaddyAvatar, picRegexStr);
                if (m.Count != 1)
                {
                    daddyAvatarValid = false;
                }
            }
            long cloudMusicId = son.CloudMusicId ?? 0;
            bool sonValid = son is not null && !string.IsNullOrEmpty(son.Name) && !string.IsNullOrEmpty(son.Daddy) && son.Name.Length <= 10 && son.Daddy.Length <= 10 && mkValid && tempValid && cloudMusicId >= 0 && avatarValid && daddyAvatarValid;
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
