namespace GameContentManager.API.Data
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string UsersCollection { get; set; } = string.Empty;
        public string CharactersCollection { get; set; } = string.Empty;
        public string AuditLogsCollection { get; set; } = string.Empty;
    }
} 