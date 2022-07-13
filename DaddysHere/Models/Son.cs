using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DaddysHere.Models
{
    public class Son
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Avatar { get; set; }
        public string Daddy { get; set; } = null!;
        public string? DaddyAvatar { get; set; }
        public string Markdown { get; set; } = null!;
        public string? Template { get; set; }
        public string? Background { get; set; }
        public string? CloudMusicId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] // 反序列化（从 WebAPI 返回）时采用本地时间
        public DateTime Expiration { get; set; }
        public bool Reserved { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Avatar: {Avatar}, Daddy: {Daddy}, DaddyAvatar: {DaddyAvatar}, Markdown: {Markdown}, Template: {Template}, Background: {Background}, CloudMusicId: {CloudMusicId}, Expiration: {Expiration}, Reserved: {Reserved}";
        }
    }
}
