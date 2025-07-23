using ChatbotFAQApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatbotFAQApi.Services
{
    public class ChatSessionService
    {
        private readonly IMongoCollection<ChatSession> _chatSessionCollection;

        public ChatSessionService(IOptions<FaqDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _chatSessionCollection = database.GetCollection<ChatSession>(settings.Value.ChatSessionCollectionName);
        }

        public async Task<ChatSession> GetBySessionIdAsync(string sessionId) =>
            await _chatSessionCollection.Find(x => x.SessionId == sessionId).FirstOrDefaultAsync();

        public async Task CreateAsync(ChatSession session) =>
            await _chatSessionCollection.InsertOneAsync(session);

        public async Task UpdateAsync(string sessionId, ChatSession session) =>
            await _chatSessionCollection.ReplaceOneAsync(x => x.SessionId == sessionId, session);
    }
} 