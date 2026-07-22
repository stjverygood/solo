using Solo.Scripts.Entities.Grasses;
using Solo.Scripts.Entities.MeleeEnemies;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Entities.Trees;
using Solo.Scripts.Global;
using System.Text.Json.Serialization;

namespace Solo.Scripts.Entities.Core
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(PlayerSaveData), nameof(PlayerSaveData))]
    [JsonDerivedType(typeof(TreeSaveData), nameof(TreeSaveData))]
    [JsonDerivedType(typeof(GrassSaveData), nameof(GrassSaveData))]
    [JsonDerivedType(typeof(MeleeEnemySaveData), nameof(MeleeEnemySaveData))]
    public class EntitySaveData
    {
        public EntityType Type { get; set; }
        public float WorldX { get; set; }
        public float WorldY { get; set; }
    }
}
