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
            _itemDataMap.Add(ItemType.MainBaseStone, new ItemData { Name = "太古源石", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/MainBaseStone.tres" });
            _itemDataMap.Add(ItemType.Silk, new ItemData { Name = "丝绸", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Leather, new ItemData { Name = "皮", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Grass, new ItemData { Name = "草", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/Grass.tres" });
            _itemDataMap.Add(ItemType.Rope, new ItemData { Name = "绳", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Stone, new ItemData { Name = "石", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/StoneItem.tres" });

            _itemDataMap.Add(ItemType.Wood, new ItemData { Name = "木", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/Wood.tres" });
            _itemDataMap.Add(ItemType.IronRaw, new ItemData { Name = "粗铁", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Iron, new ItemData { Name = "铁块", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.CopperRaw, new ItemData { Name = "粗铜", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Copper, new ItemData { Name = "铜块", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.SilverRaw, new ItemData { Name = "粗银", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Silver, new ItemData { Name = "银块", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });
            _itemDataMap.Add(ItemType.Gold, new ItemData { Name = "黄金", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/milk_icon.tres" });


            //木制套装
            _itemDataMap.Add(ItemType.WoodSword, new ItemData { Name = "木剑", IconPath = "res://Assets/AtlasTextures/WoodSword.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.WoodBow, new ItemData { Name = "木弓", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.WoodPickaxe, new ItemData { Name = "木稿", IconPath = "res://Assets/AtlasTextures/WoodPickaxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.WoodAxe, new ItemData { Name = "木斧", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.WoodPot, new ItemData { Name = "木壶", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.WoodRod, new ItemData { Name = "木竿", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.WoodHelmet, new ItemData { Name = "木盔", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.WoodArmor, new ItemData { Name = "木甲", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.WoodBoot, new ItemData { Name = "木鞋", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });

            //铁制套装
            _itemDataMap.Add(ItemType.IronSword, new ItemData { Name = "铁剑", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.IronBow, new ItemData { Name = "铁弓", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.IronPickaxe, new ItemData { Name = "铁稿", IconPath = "res://Assets/AtlasTextures/milk_icon.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.IronAxe, new ItemData { Name = "铁斧", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.IronPot, new ItemData { Name = "铁壶", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.IronRod, new ItemData { Name = "铁竿", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.IronHelmet, new ItemData { Name = "铁盔", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.IronArmor, new ItemData { Name = "铁甲", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.IronBoot, new ItemData { Name = "铁鞋", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });

            //金制装备
            _itemDataMap.Add(ItemType.GoldSword, new ItemData { Name = "金剑", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.GoldBow, new ItemData { Name = "金弓", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.GoldPickaxe, new ItemData { Name = "金稿", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.GoldAxe, new ItemData { Name = "金斧", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.GoldPot, new ItemData { Name = "金壶", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.GoldRod, new ItemData { Name = "金竿", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.GoldHelmet, new ItemData { Name = "金盔", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.GoldArmor, new ItemData { Name = "金甲", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.GoldBoot, new ItemData { Name = "金鞋", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });

            //玉制套装
            _itemDataMap.Add(ItemType.JadeSword, new ItemData { Name = "玉剑", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.JadeBow, new ItemData { Name = "玉弓", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.JadePickaxe, new ItemData { Name = "玉稿", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.JadeAxe, new ItemData { Name = "玉斧", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.JadePot, new ItemData { Name = "玉壶", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.JadeRod, new ItemData { Name = "玉竿", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.JadeHelmet, new ItemData { Name = "玉盔", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.JadeArmor, new ItemData { Name = "玉甲", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });
            _itemDataMap.Add(ItemType.JadeBoot, new ItemData { Name = "玉鞋", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", MaxDur = 100 });

            //建筑
            _itemDataMap.Add(ItemType.MainBase, new ItemData { Name = "聚灵之源", MaxCount = 99, IconPath = "res://Assets/AtlasTextures/MainBase.tres", isBuilding = true });
            _itemDataMap.Add(ItemType.Flag, new ItemData { Name = "法旗", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", isBuilding = true });
            _itemDataMap.Add(ItemType.BuildingCraft, new ItemData { Name = "天工殿", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", isBuilding = true });
            _itemDataMap.Add(ItemType.ToolCraft, new ItemData { Name = "锻器台", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", isBuilding = true });
            _itemDataMap.Add(ItemType.ArmorCraft, new ItemData { Name = "织锦阁", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", isBuilding = true });
            _itemDataMap.Add(ItemType.ItemBox, new ItemData { Name = "乾坤箱", IconPath = "res://Assets/AtlasTextures/WoodAxe.tres", isBuilding = true });
        }

        public ItemData GetItemData(ItemType type)
        {
            return _itemDataMap[type];
        }
    }
}
