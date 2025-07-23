namespace ChatbotFAQApi.Models
{
    public class FaqDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string FaqCollectionName { get; set; }
        public string ChatSessionCollectionName { get; set; }
    }
}
