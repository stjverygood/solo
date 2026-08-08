using Solo.Scripts.Components.ExpComponents;
using Solo.Scripts.Components.HpComponents;
using Solo.Scripts.Components.InventoryComponents;
using Solo.Scripts.Components.PickableComponents;
using Solo.Scripts.Components.PositionComponents;
using Solo.Scripts.Components.QiComponents;
using Solo.Scripts.Components.RealmComponents;
using Solo.Scripts.Components.StartPositionComponents;
using System.Text.Json.Serialization;

namespace Solo.Scripts.Components.Core
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(ExpComponentSaveData), nameof(ExpComponentSaveData))]
    [JsonDerivedType(typeof(HpComponentSaveData), nameof(HpComponentSaveData))]
    [JsonDerivedType(typeof(InventoryComponentSaveData), nameof(InventoryComponentSaveData))]
    [JsonDerivedType(typeof(PositionComponentSaveData), nameof(PositionComponentSaveData))]
    [JsonDerivedType(typeof(QiComponentSaveData), nameof(QiComponentSaveData))]
    [JsonDerivedType(typeof(RealmComponentSaveData), nameof(RealmComponentSaveData))]
    [JsonDerivedType(typeof(StartPositionComponentSaveData), nameof(StartPositionComponentSaveData))]
    [JsonDerivedType(typeof(PickableComponentSaveData), nameof(PickableComponentSaveData))]
    public class ComponentSaveData
    {
        public required string ComponentName { get; set; }
    }
}
