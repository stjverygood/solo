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
            _itemDataMap.Add(ItemType.MainBaseStone, new ItemData("太古源石", 1, "res://Assets/AtlasTextures/MainBaseStone.tres", -1));
            _itemDataMap.Add(ItemType.Silk, new ItemData("丝绸", 99, "res://Assets/AtlasTextures/milk_icon.tres", -1));
            _itemDataMap.Add(ItemType.Leather, new ItemData("皮", 99, "res://Assets/AtlasTextures/milk_icon.tres", -1));
            _itemDataMap.Add(ItemType.Grass, new ItemData("草", 99, "res://Assets/AtlasTextures/milk_icon.tres", -1));
            _itemDataMap.Add(ItemType.Rope, new ItemData("绳", 99, "res://Assets/AtlasTextures/milk_icon.tres", -1));
            _itemDataMap.Add(ItemType.Stone, new ItemData("石", 99, "res://Assets/AtlasTextures/milk_icon.tres", -1));

            _itemDataMap.Add(ItemType.Wood, new ItemData("木", 99, "res://Assets/AtlasTextures/milk_icon.tres", -1));
            _itemDataMap.Add(ItemType.IronRaw, new ItemData("粗铁", 16, "res://Assets/AtlasTextures/milk_icon.tres", -1));
            _itemDataMap.Add(ItemType.Iron, new ItemData("铁块", 16, "res://Assets/AtlasTextures/milk_icon.tres", -1));
            _itemDataMap.Add(ItemType.CopperRaw, new ItemData("粗铜", 16, "res://Assets/AtlasTextures/milk_icon.tres", -1));
            _itemDataMap.Add(ItemType.Copper, new ItemData("铜块", 16, "res://Assets/AtlasTextures/milk_icon.tres", -1));
            _itemDataMap.Add(ItemType.SilverRaw, new ItemData("粗银", 16, "res://Assets/AtlasTextures/milk_icon.tres", -1));
            _itemDataMap.Add(ItemType.Silver, new ItemData("银块", 16, "res://Assets/AtlasTextures/milk_icon.tres", -1));
            _itemDataMap.Add(ItemType.Gold, new ItemData("黄金", 16, "res://Assets/AtlasTextures/milk_icon.tres", -1));


            //木制套装
            _itemDataMap.Add(ItemType.WoodSword, new ItemData("木剑", 1, "res://Assets/AtlasTextures/WoodSword.tres", 100));
            _itemDataMap.Add(ItemType.WoodBow, new ItemData("木弓", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.WoodPickaxe, new ItemData("木稿", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.WoodAxe, new ItemData("木斧", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.WoodPot, new ItemData("木壶", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.WoodRod, new ItemData("木竿", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.WoodHelmet, new ItemData("木盔", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.WoodArmor, new ItemData("木甲", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.WoodBoot, new ItemData("木鞋", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));

            //铁制套装
            _itemDataMap.Add(ItemType.IronSword, new ItemData("铁剑", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.IronBow, new ItemData("铁弓", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.IronPickaxe, new ItemData("铁稿", 1, "res://Assets/AtlasTextures/milk_icon.tres", 100));
            _itemDataMap.Add(ItemType.IronAxe, new ItemData("铁斧", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.IronPot, new ItemData("铁壶", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.IronRod, new ItemData("铁竿", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.IronHelmet, new ItemData("铁盔", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.IronArmor, new ItemData("铁甲", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.IronBoot, new ItemData("铁鞋", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));

            //金制装备
            _itemDataMap.Add(ItemType.GoldSword, new ItemData("金剑", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.GoldBow, new ItemData("金弓", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.GoldPickaxe, new ItemData("金稿", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.GoldAxe, new ItemData("金斧", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.GoldPot, new ItemData("金壶", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.GoldRod, new ItemData("金竿", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.GoldHelmet, new ItemData("金盔", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.GoldArmor, new ItemData("金甲", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.GoldBoot, new ItemData("金鞋", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));

            //玉制套装
            _itemDataMap.Add(ItemType.JadeSword, new ItemData("玉剑", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.JadeBow, new ItemData("玉弓", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.JadePickaxe, new ItemData("玉稿", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.JadeAxe, new ItemData("玉斧", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.JadePot, new ItemData("玉壶", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.JadeRod, new ItemData("玉竿", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.JadeHelmet, new ItemData("玉盔", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.JadeArmor, new ItemData("玉甲", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));
            _itemDataMap.Add(ItemType.JadeBoot, new ItemData("玉鞋", 1, "res://Assets/AtlasTextures/WoodAxe.tres", 100));

            //建筑
            _itemDataMap.Add(ItemType.MainBase, new ItemData("聚灵之源", 1, "res://Assets/AtlasTextures/WoodAxe.tres", -1));
            _itemDataMap.Add(ItemType.Flag, new ItemData("法旗", 1, "res://Assets/AtlasTextures/WoodAxe.tres", -1));
            _itemDataMap.Add(ItemType.BuildingCraft, new ItemData("天工殿", 1, "res://Assets/AtlasTextures/WoodAxe.tres", -1));
            _itemDataMap.Add(ItemType.ToolCraft, new ItemData("锻器台", 1, "res://Assets/AtlasTextures/WoodAxe.tres", -1));
            _itemDataMap.Add(ItemType.ArmorCraft, new ItemData("织锦阁", 1, "res://Assets/AtlasTextures/WoodAxe.tres", -1));
            _itemDataMap.Add(ItemType.ItemBox, new ItemData("乾坤箱", 1, "res://Assets/AtlasTextures/WoodAxe.tres", -1));
        }

        public ItemData GetItemData(ItemType type)
        {
            return _itemDataMap[type];
        }
    }
}
