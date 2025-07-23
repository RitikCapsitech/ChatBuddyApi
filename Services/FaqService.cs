using ChatbotFAQApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ChatbotFAQApi.Services
{
    public class FaqService
    {
        private readonly IMongoCollection<FaqItem> _faqCollection;

        public FaqService(IOptions<FaqDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _faqCollection = database.GetCollection<FaqItem>(settings.Value.FaqCollectionName);
        }

        public async Task<List<FaqItem>> GetAsync() =>
            await _faqCollection.Find(_ => true).ToListAsync();

        public async Task CreateAsync(FaqItem faq) =>
            await _faqCollection.InsertOneAsync(faq);

        public async Task CreateManyAsync(List<FaqItem> faqs) =>
            await _faqCollection.InsertManyAsync(faqs);

        public async Task DeleteAsync(FaqItem faq) =>
            await _faqCollection.DeleteOneAsync(x => x.Id == faq.Id);

        public async Task<FaqItem?> GetByIdAsync(string id) =>
            await _faqCollection.Find(f => f.Id == id).FirstOrDefaultAsync();

        public async Task DeleteAsync(string id) =>
            await _faqCollection.DeleteOneAsync(f => f.Id == id);

        public async Task UpdateAsync(string id, FaqItem updatedFaq) =>
            await _faqCollection.ReplaceOneAsync(f => f.Id == id, updatedFaq);

        public async Task DeleteAllAsync() =>
            await _faqCollection.DeleteManyAsync(_ => true);


    }
}
