using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace GameContentManager.API.Models
{
    public class GameCharacter
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string Name { get; set; }

        [BsonRequired]
        public string Description { get; set; }

        [BsonRequired]
        public string Rarity { get; set; }

        [BsonRequired]
        public int UpgradeLevel { get; set; }

        [BsonRequired]
        public int SortOrder { get; set; }

        [BsonRequired]
        public string ImageUrl { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public enum CharacterRarity
    {
        Common,
        Rare,
        Epic
    }
} 