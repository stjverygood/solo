using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.System.CraftSystem
{
    public class CraftItem
    {
        public ItemType Type;
        public List<(ItemType, int)> RequiredItemList;
        public CraftItem(ItemType type, List<(ItemType, int)> requiredItemList)
        {
            Type = type;
            RequiredItemList = requiredItemList;
        }
    }
}
