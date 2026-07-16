using Solo.Scripts.Global;
using System.Text.Json.Serialization;

namespace Solo.Scripts.Entities
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(TreeSaveData), nameof(EntityType.Tree))]
    public class EntitySaveData
    {
        public EntityType Type { get; set; }
        public float WorldX { get; set; }
        public float WorldY { get; set; }
    }
}
