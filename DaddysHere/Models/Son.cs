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
        public string? Markdown { get; set; }
        public long? CloudMusicId { get; set; }
        public string? Template { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] // 反序列化（从 WebAPI 返回）时采用本地时间
        public DateTime Expiration { get; set; }
        public bool Reserved { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Daddy: {Daddy}, Markdown: {Markdown}, Template: {Template}, Expiration: {Expiration}, Reserved: {Reserved}";
        }
    }
}
