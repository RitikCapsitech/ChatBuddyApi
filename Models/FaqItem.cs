using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ChatbotFAQApi.Models
{
    public class FaqItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("query")]
        [BsonRequired]
        public string Query { get; set; }

        [BsonElement("response")]
        [BsonRequired]
        public string Response { get; set; }

        [BsonElement("options")]
        [BsonIgnoreIfNull]
        public List<FaqOption> Options { get; set; } = new List<FaqOption>();
    }

    public class FaqOption
    {
        [BsonElement("subId")]
        [BsonRequired]
        public string SubId { get; set; }

        [BsonElement("optionText")]
        [BsonRequired]
        public string OptionText { get; set; }

        [BsonElement("response")]
        [BsonRequired]
        public string Response { get; set; }
    }

    public class FaqBulkRequest
    {
        public List<FaqItem> Items { get; set; }
    }
}
