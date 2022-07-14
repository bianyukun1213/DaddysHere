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
            const long LIMIT_VALUE = 30000;
            long estimatedCount = await _sonsCollection.EstimatedDocumentCountAsync();
            return estimatedCount >= LIMIT_VALUE;
        }
        public async Task<bool> DoesSonsCountReachLimitValueAsync(Son son)
        {
            const long LIMIT_VALUE = 25;
            long count = await _sonsCollection.CountDocumentsAsync(s => s.Name == son.Name);
            return count >= LIMIT_VALUE;
        }
        public async Task<bool> IsSonNameUniqueAsync(string name)
        {
            return await _sonsCollection.Find(s => s.Name == name && s.NameUnique).FirstOrDefaultAsync() is not null;
        }
        public async Task<bool> IsSonProtectedAsync(string id)
        {
            Son? son = await GetSonByIdAsync(id);
            return son is not null && son.Protected;
        }
        public bool IsSonValid(Son son)
        {
            bool avatarValid = true;
            bool daddyAvatarValid = true;
            bool backgroundValid = true;
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
            if (!string.IsNullOrEmpty(son.Background))
            {
                var m = Regex.Matches(son.Background, picRegexStr);
                if (m.Count != 1)
                {
                    backgroundValid = false;
                }
            }
            //long cloudMusicId = son.CloudMusicId ?? 0;
            bool cloudMusicIdValid = (son.CloudMusicId?.Length ?? 0) <= 24;
            // 名字（<=16 字）、爹名（<=16 字）、Markdown（<=1000 字）、模板名（<=16 字）必须
            bool sonValid = son is not null && !string.IsNullOrEmpty(son.Name) && !string.IsNullOrEmpty(son.Daddy) && !string.IsNullOrEmpty(son.Template) && son.Name.Length <= 10 && son.Daddy.Length <= 10 && Enum.IsDefined<Gender>(son.Gender) && son.Markdown.Length <= 1000 && son.Template.Length <= 10 && avatarValid && daddyAvatarValid && backgroundValid && cloudMusicIdValid;
            return sonValid;
        }
        public void DeleteExpiredSons()
        {
            _logger.LogInformation("正在删除过期儿子。");
            var res = _sonsCollection.DeleteMany(s => s.Expiration <= DateTime.Now.Date && !s.Reserved && !s.Protected);
            _logger.LogInformation("删除完成，共删除 {count} 个过期儿子。", res.DeletedCount);
        }
    }
}
