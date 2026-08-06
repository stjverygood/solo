using Solo.Scripts.Entities.Core;
using Solo.Scripts.System.ItemSystem;

namespace Solo.Scripts.Entities.Components.PickableComponents
{
    public class PickableComponentSaveData : ComponentSaveData
    {
        public required ItemInstance ItemInstance { get; set; }
    }
}
