using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.System.ItemSystem
{
    public class ItemManager
    {
        private static ItemManager _instance;
        public static ItemManager Instance => _instance ??= new ItemManager();
        private Dictionary<ItemType, ItemData> _itemDataMap = new Dictionary<ItemType, ItemData>();

        private ItemManager()
        {
            //材料
            _itemDataMap.Add(ItemType.Wood, new ItemData() { Type = ItemType.Wood, MaxCount = 16, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.Stone, new ItemData() { Type = ItemType.Stone, MaxCount = 16, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.Iron, new ItemData() { Type = ItemType.Iron, MaxCount = 16, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.Gold, new ItemData() { Type = ItemType.Gold, MaxCount = 16, IconPath = "res://Assets/icon.svg" });

            //木制装备
            _itemDataMap.Add(ItemType.WoodSword, new ItemData() { Type = ItemType.WoodSword, MaxCount = 1 });
            _itemDataMap.Add(ItemType.WoodBow, new ItemData() { Type = ItemType.WoodBow, MaxCount = 1 });
            _itemDataMap.Add(ItemType.WoodPickaxe, new ItemData() { Type = ItemType.WoodPickaxe, MaxCount = 1 });
            _itemDataMap.Add(ItemType.WoodAxe, new ItemData() { Type = ItemType.WoodAxe, MaxCount = 1 });
            _itemDataMap.Add(ItemType.PotAxe, new ItemData() { Type = ItemType.PotAxe, MaxCount = 1 });
            _itemDataMap.Add(ItemType.WoodHelmet, new ItemData() { Type = ItemType.WoodHelmet, MaxCount = 1 });
            _itemDataMap.Add(ItemType.WoodArmor, new ItemData() { Type = ItemType.WoodArmor, MaxCount = 1 });
            _itemDataMap.Add(ItemType.WoodBoot, new ItemData() { Type = ItemType.WoodBoot, MaxCount = 1 });

            //石制装备
            _itemDataMap.Add(ItemType.StoneSword, new ItemData() { Type = ItemType.StoneSword, MaxCount = 1 });
            _itemDataMap.Add(ItemType.StoneBow, new ItemData() { Type = ItemType.StoneBow, MaxCount = 1 });
            _itemDataMap.Add(ItemType.StonePickaxe, new ItemData() { Type = ItemType.StonePickaxe, MaxCount = 1 });
            _itemDataMap.Add(ItemType.StoneAxe, new ItemData() { Type = ItemType.StoneAxe, MaxCount = 1 });
            _itemDataMap.Add(ItemType.StonePot, new ItemData() { Type = ItemType.StonePot, MaxCount = 1 });
            _itemDataMap.Add(ItemType.StoneHelmet, new ItemData() { Type = ItemType.StoneHelmet, MaxCount = 1 });
            _itemDataMap.Add(ItemType.StoneArmor, new ItemData() { Type = ItemType.StoneArmor, MaxCount = 1 });
            _itemDataMap.Add(ItemType.StoneBoot, new ItemData() { Type = ItemType.StoneBoot, MaxCount = 1 });

            //铁制装备
            _itemDataMap.Add(ItemType.IronSword, new ItemData() { Type = ItemType.IronSword, MaxCount = 1 });
            _itemDataMap.Add(ItemType.IronBow, new ItemData() { Type = ItemType.IronBow, MaxCount = 1 });
            _itemDataMap.Add(ItemType.IronPickaxe, new ItemData() { Type = ItemType.IronPickaxe, MaxCount = 1 });
            _itemDataMap.Add(ItemType.IronAxe, new ItemData() { Type = ItemType.IronAxe, MaxCount = 1 });
            _itemDataMap.Add(ItemType.IronPot, new ItemData() { Type = ItemType.IronPot, MaxCount = 1 });
            _itemDataMap.Add(ItemType.IronHelmet, new ItemData() { Type = ItemType.IronHelmet, MaxCount = 1 });
            _itemDataMap.Add(ItemType.IronArmor, new ItemData() { Type = ItemType.IronArmor, MaxCount = 1 });
            _itemDataMap.Add(ItemType.IronBoot, new ItemData() { Type = ItemType.IronBoot, MaxCount = 1 });

            //金制装备
            _itemDataMap.Add(ItemType.GoldSword, new ItemData() { Type = ItemType.GoldSword, MaxCount = 1 });
            _itemDataMap.Add(ItemType.GoldBow, new ItemData() { Type = ItemType.GoldBow, MaxCount = 1 });
            _itemDataMap.Add(ItemType.GoldPickaxe, new ItemData() { Type = ItemType.GoldPickaxe, MaxCount = 1 });
            _itemDataMap.Add(ItemType.GoldAxe, new ItemData() { Type = ItemType.GoldAxe, MaxCount = 1 });
            _itemDataMap.Add(ItemType.GoldPot, new ItemData() { Type = ItemType.GoldPot, MaxCount = 1 });
            _itemDataMap.Add(ItemType.GoldHelmet, new ItemData() { Type = ItemType.GoldHelmet, MaxCount = 1 });
            _itemDataMap.Add(ItemType.GoldArmor, new ItemData() { Type = ItemType.GoldArmor, MaxCount = 1 });
            _itemDataMap.Add(ItemType.GoldBoot, new ItemData() { Type = ItemType.GoldBoot, MaxCount = 1 });
        }

        public ItemData GetItemData(ItemType type)
        {
            return _itemDataMap[type];
        }
    }
}
