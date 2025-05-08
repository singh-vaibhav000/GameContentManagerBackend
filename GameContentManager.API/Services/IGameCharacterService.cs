using GameContentManager.API.Models;

namespace GameContentManager.API.Services
{
    public interface IGameCharacterService
    {
        Task<IEnumerable<GameCharacter>> GetAllCharactersAsync();
        Task<GameCharacter> GetCharacterByIdAsync(string id);
        Task<List<GameCharacter>> SearchCharacters(string searchTerm);
        Task<GameCharacter> CreateCharacterAsync(GameCharacter character, string userId);
        Task UpdateCharacterAsync(string id, GameCharacter character,string userId);
        Task DeleteCharacterAsync(string id, string userId);
    }
} 