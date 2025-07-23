using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ChatbotFAQApi.Models
{
    public class ChatSession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("sessionId")]
        public string SessionId { get; set; }

        [BsonElement("messages")]
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ChatMessage
    {
        [BsonElement("sender")]
        public string Sender { get; set; } // "user" or "bot"

        [BsonElement("text")]
        public string Text { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 