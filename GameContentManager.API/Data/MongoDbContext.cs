using GameContentManager.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameContentManager.API.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);

            Users = _database.GetCollection<User>(settings.Value.UsersCollection);
            GameCharacters = _database.GetCollection<GameCharacter>(settings.Value.CharactersCollection);
            AuditLogs = _database.GetCollection<AuditLog>(settings.Value.AuditLogsCollection);

            CreateIndexes();
        }

        public IMongoCollection<User> Users { get; }
        public IMongoCollection<GameCharacter> GameCharacters { get; }
        public IMongoCollection<AuditLog> AuditLogs { get; }

        private void CreateIndexes()
        {
            var userEmailIndex = Builders<User>.IndexKeys.Ascending(u => u.Email);
            Users.Indexes.CreateOne(new CreateIndexModel<User>(userEmailIndex, new CreateIndexOptions { Unique = true }));

            var characterNameIndex = Builders<GameCharacter>.IndexKeys.Ascending(c => c.Name);
            GameCharacters.Indexes.CreateOne(new CreateIndexModel<GameCharacter>(characterNameIndex));

            var auditLogTimestampIndex = Builders<AuditLog>.IndexKeys.Descending(a => a.Timestamp);
            AuditLogs.Indexes.CreateOne(new CreateIndexModel<AuditLog>(auditLogTimestampIndex));
        }
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string UsersCollection { get; set; } = string.Empty;
        public string CharactersCollection { get; set; } = string.Empty;
        public string AuditLogsCollection { get; set; } = string.Empty;
    }
} 