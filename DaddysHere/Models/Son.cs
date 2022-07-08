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
        public string Daddy { get; set; } = null!;
        public string? Markdown { get; set; }
        public string? Theme { get; set; }
    }
}
