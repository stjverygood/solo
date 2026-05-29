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
            _itemDataMap.Add(ItemType.Wood, new ItemData() { MaxCount = 16, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.Stone, new ItemData() { MaxCount = 16, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.IronRaw, new ItemData() { MaxCount = 16, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.Iron, new ItemData() { MaxCount = 16, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.CopperRaw, new ItemData() { MaxCount = 16, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.Copper, new ItemData() { MaxCount = 16, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.SilverRaw, new ItemData() { MaxCount = 16, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.Silver, new ItemData() { MaxCount = 16, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.Gold, new ItemData() { MaxCount = 16, IconPath = "res://Assets/icon.svg" });

            //木制套装
            _itemDataMap.Add(ItemType.WoodSword, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.WoodBow, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.WoodPickaxe, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.WoodAxe, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.WoodPot, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.WoodFishingRod, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.WoodHelmet, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.WoodArmor, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.WoodBoot, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });

            //铁制套装
            _itemDataMap.Add(ItemType.IronSword, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.IronBow, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.IronPickaxe, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.IronAxe, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.IronPot, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.IronFishingRod, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.IronHelmet, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.IronArmor, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.IronBoot, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });

            //铜制套装
            _itemDataMap.Add(ItemType.CopperSword, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.CopperBow, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.CopperPickaxe, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.CopperAxe, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.CopperPot, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.CopperFishingRod, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.CopperHelmet, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.CopperArmor, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.CopperBoot, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });

            //银制套装
            _itemDataMap.Add(ItemType.SilverSword, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.SilverBow, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.SilverPickaxe, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.SilverAxe, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.SilverPot, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.SilverFishingRod, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.SilverHelmet, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.SilverArmor, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.SilverBoot, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });

            //金制装备
            _itemDataMap.Add(ItemType.GoldSword, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.GoldBow, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.GoldPickaxe, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.GoldAxe, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.GoldPot, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.GoldFishingRod, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.GoldHelmet, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.GoldArmor, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
            _itemDataMap.Add(ItemType.GoldBoot, new ItemData() { MaxCount = 1, IconPath = "res://Assets/icon.svg" });
        }

        public ItemData GetItemData(ItemType type)
        {
            return _itemDataMap[type];
        }
    }
}
