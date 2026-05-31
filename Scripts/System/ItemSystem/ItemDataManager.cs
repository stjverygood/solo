using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.System.ItemSystem
{
    public class ItemDataManager
    {
        private static ItemDataManager _instance;
        public static ItemDataManager Instance => _instance ??= new ItemDataManager();
        private Dictionary<ItemType, ItemData> _itemDataMap = new Dictionary<ItemType, ItemData>();

        private ItemDataManager()
        {
            //材料
            _itemDataMap.Add(ItemType.Silk, new ItemData() { Name = "丝绸", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Leather, new ItemData() { Name = "皮", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Grass, new ItemData() { Name = "草", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Rope, new ItemData() { Name = "绳", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Stone, new ItemData() { Name = "石头", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });

            _itemDataMap.Add(ItemType.Wood, new ItemData() { Name = "木头", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.IronRaw, new ItemData() { Name = "粗铁", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Iron, new ItemData() { Name = "铁块", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.CopperRaw, new ItemData() { Name = "粗铜", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Copper, new ItemData() { Name = "铜块", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.SilverRaw, new ItemData() { Name = "粗银", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Silver, new ItemData() { Name = "银块", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Gold, new ItemData() { Name = "黄金", MaxCount = 16, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });


            //木制套装
            _itemDataMap.Add(ItemType.WoodSword, new ItemData() { Name = "木剑", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodSword.tres" });
            _itemDataMap.Add(ItemType.WoodBow, new ItemData() { Name = "木弓", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.WoodPickaxe, new ItemData() { Name = "木稿", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.WoodAxe, new ItemData() { Name = "木斧", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.WoodPot, new ItemData() { Name = "木壶", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.WoodFishingRod, new ItemData() { Name = "木竿", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.WoodHelmet, new ItemData() { Name = "木盔", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.WoodArmor, new ItemData() { Name = "木甲", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.WoodBoot, new ItemData() { Name = "木鞋", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });

            //铁制套装
            _itemDataMap.Add(ItemType.IronSword, new ItemData() { Name = "铁剑", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.IronBow, new ItemData() { Name = "铁弓", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.IronPickaxe, new ItemData() { Name = "铁稿", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.IronAxe, new ItemData() { Name = "铁斧", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.IronPot, new ItemData() { Name = "铁壶", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.IronFishingRod, new ItemData() { Name = "铁竿", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.IronHelmet, new ItemData() { Name = "铁盔", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.IronArmor, new ItemData() { Name = "铁甲", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.IronBoot, new ItemData() { Name = "铁鞋", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });

            //金制装备
            _itemDataMap.Add(ItemType.GoldSword, new ItemData() { Name = "金剑", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.GoldBow, new ItemData() { Name = "金弓", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.GoldPickaxe, new ItemData() { Name = "金稿", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.GoldAxe, new ItemData() { Name = "金斧", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.GoldPot, new ItemData() { Name = "金壶", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.GoldFishingRod, new ItemData() { Name = "金竿", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.GoldHelmet, new ItemData() { Name = "金盔", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.GoldArmor, new ItemData() { Name = "金甲", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.GoldBoot, new ItemData() { Name = "金鞋", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });

            //玉制套装
            _itemDataMap.Add(ItemType.JadeSword, new ItemData() { Name = "玉剑", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.JadeBow, new ItemData() { Name = "玉弓", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.JadePickaxe, new ItemData() { Name = "玉稿", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.JadeAxe, new ItemData() { Name = "玉斧", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.JadePot, new ItemData() { Name = "玉壶", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.JadeFishingRod, new ItemData() { Name = "玉竿", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.JadeHelmet, new ItemData() { Name = "玉盔", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.JadeArmor, new ItemData() { Name = "玉甲", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
            _itemDataMap.Add(ItemType.JadeBoot, new ItemData() { Name = "玉鞋", MaxCount = 1, IconPath = "res://Assets/AtlasTextures/WoodAxe.tres" });
        }

        public ItemData GetItemData(ItemType type)
        {
            return _itemDataMap[type];
        }
    }
}
