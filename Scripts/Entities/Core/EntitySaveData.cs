using Solo.Scripts.Entities.DropItems;
using Solo.Scripts.Entities.Enemies;
using Solo.Scripts.Entities.Expballs;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Entities.Resources;
using Solo.Scripts.Global;
using System.Text.Json.Serialization;

namespace Solo.Scripts.Entities.Core
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(PlayerSaveData), nameof(PlayerSaveData))]
    [JsonDerivedType(typeof(ResourceSaveData), nameof(ResourceSaveData))]
    [JsonDerivedType(typeof(EnemySaveData), nameof(EnemySaveData))]
    [JsonDerivedType(typeof(DropItemSaveData), nameof(DropItemSaveData))]
    [JsonDerivedType(typeof(ExpBallSaveData), nameof(ExpBallSaveData))]
    public class EntitySaveData
    {
        public EntityType Type { get; set; }
        public float WorldX { get; set; }
        public float WorldY { get; set; }
    }
}
