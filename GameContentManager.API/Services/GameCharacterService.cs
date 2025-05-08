using System.Collections.Generic;
using System.Threading.Tasks;
using GameContentManager.API.Data;
using GameContentManager.API.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GameContentManager.API.Services
{
    public class GameCharacterService : IGameCharacterService
    {
        private readonly IMongoCollection<GameCharacter> _characters;
        private readonly IAuditLogService _auditLogService;

        public GameCharacterService(MongoDbContext context, IAuditLogService auditLogService)
        {
            _characters = context.GameCharacters;
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<GameCharacter>> GetAllCharactersAsync()
        {
            return await _characters.Find(_ => true)
                .SortBy(c => c.SortOrder)
                .ToListAsync();
        }

        public async Task<GameCharacter> GetCharacterByIdAsync(string id)
        {
            return await _characters.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<GameCharacter>> SearchCharacters(string searchTerm)
        {
            var filter = Builders<GameCharacter>.Filter.Or(
                Builders<GameCharacter>.Filter.Regex(c => c.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<GameCharacter>.Filter.Regex(c => c.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );

            return await _characters.Find(filter)
                .SortBy(c => c.SortOrder)
                .ToListAsync();
        }

        public async Task<GameCharacter> CreateCharacterAsync(GameCharacter character, string userId)
        {
            character.CreatedAt = DateTime.UtcNow;
            character.CreatedBy = userId;
            character.UpdatedAt = DateTime.UtcNow;
            character.UpdatedBy = userId;

            await _characters.InsertOneAsync(character);
            await _auditLogService.LogActionAsync(userId, "Create", "GameCharacter", character.Id, $"Created character {character.Name}", null,character.Description);
            return character;
        }

        public async Task UpdateCharacterAsync(string id, GameCharacter character, string userId)
        {
            var characterdetails = await GetCharacterByIdAsync(id);
            character.UpdatedAt = DateTime.UtcNow;
            character.UpdatedBy = userId;
            character.Id = id;
            await _characters.ReplaceOneAsync(c => c.Id == id, character);
            await _auditLogService.LogActionAsync(character.UpdatedBy, "Update", id,"GameCharacter", $"Updated character {character.Name}", characterdetails.Description, character.Description);
        }

        public async Task DeleteCharacterAsync(string id, string userId)
        {
            var characterdetails = await GetCharacterByIdAsync(id);
            var character = await GetCharacterByIdAsync(id);
            if (character != null)
            {
                await _characters.DeleteOneAsync(c => c.Id == id);
               
                await _auditLogService.LogActionAsync(userId, "Delete", "GameCharacter", id, $"Deleted character {character.Name}", characterdetails.Description, character.Description);
            }
        }
    }
} 