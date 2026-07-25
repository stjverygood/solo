using Solo.Scripts.Entities.Core;
using Solo.Scripts.System.ItemSystem;

namespace Solo.Scripts.Entities.DropItems
{
    public class DropItemSaveData : EntitySaveData
    {
        public required ItemInstance ItemInstance { get; set; }
    }
}
