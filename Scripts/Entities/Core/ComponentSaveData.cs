using Solo.Scripts.Entities.Components.ExpComponents;
using Solo.Scripts.Entities.Components.HpComponents;
using Solo.Scripts.Entities.Components.InventoryComponents;
using Solo.Scripts.Entities.Components.PositionComponents;
using Solo.Scripts.Entities.Components.QiComponents;
using Solo.Scripts.Entities.Components.RealmComponents;
using Solo.Scripts.Entities.Components.StartPositionComponents;
using System.Text.Json.Serialization;

namespace Solo.Scripts.Entities.Core
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(ExpComponentSaveData), nameof(ExpComponentSaveData))]
    [JsonDerivedType(typeof(HpComponentSaveData), nameof(HpComponentSaveData))]
    [JsonDerivedType(typeof(InventoryComponentSaveData), nameof(InventoryComponentSaveData))]
    [JsonDerivedType(typeof(PositionComponentSaveData), nameof(PositionComponentSaveData))]
    [JsonDerivedType(typeof(QiComponentSaveData), nameof(QiComponentSaveData))]
    [JsonDerivedType(typeof(RealmComponentSaveData), nameof(RealmComponentSaveData))]
    [JsonDerivedType(typeof(StartPositionComponentSaveData), nameof(StartPositionComponentSaveData))]
    public class ComponentSaveData
    {
        public required string TypeName { get; set; }
    }
}
