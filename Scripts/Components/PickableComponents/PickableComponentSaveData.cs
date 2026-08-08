using Solo.Scripts.Components.Core;
using Solo.Scripts.System.ItemSystem;

namespace Solo.Scripts.Components.PickableComponents
{
    public class PickableComponentSaveData : ComponentSaveData
    {
        public required ItemInstance ItemInstance { get; set; }
    }
}
