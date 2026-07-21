using Solo.Scripts.Entities.Grasses;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Entities.Trees;
using Solo.Scripts.Global;
using System.Text.Json.Serialization;

namespace Solo.Scripts.Entities.Core
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(PlayerSaveData), nameof(EntityType.Player))]
    [JsonDerivedType(typeof(TreeSaveData), nameof(EntityType.Tree))]
    [JsonDerivedType(typeof(GrassSaveData), nameof(EntityType.Grass))]
    public class EntitySaveData
    {
        public EntityType Type { get; set; }
        public float WorldX { get; set; }
        public float WorldY { get; set; }
    }
}
