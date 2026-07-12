using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.System.ItemSystem
{
    public class ItemDataManager
    {
        private static ItemDataManager _instance;
        public static ItemDataManager Instance => _instance ??= new ItemDataManager();
        private Dictionary<ItemType, ItemData> _dataMap = new Dictionary<ItemType, ItemData>();

        private ItemDataManager()
        {
            #region 材料
            _dataMap.Add(ItemType.MainBaseStone, new ItemData
            {
                Name = "太古源石",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/MainBaseStone.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            _dataMap.Add(ItemType.Silk, new ItemData
            {
                Name = "丝绸",
                MaxCount = 99,
                IconPath =
                "res://Assets/AtlasTextures/milk_icon.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            _dataMap.Add(ItemType.Leather, new ItemData
            {
                Name = "皮",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/milk_icon.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            _dataMap.Add(ItemType.Grass, new ItemData
            {
                Name = "草",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/Grass.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            _dataMap.Add(ItemType.Rope, new ItemData
            {
                Name = "绳",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/Rope.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Grass, 1),
                },
            });
            _dataMap.Add(ItemType.Stone, new ItemData
            {
                Name = "石",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/StoneItem.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });

            _dataMap.Add(ItemType.Wood, new ItemData
            {
                Name = "木",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/Wood.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            _dataMap.Add(ItemType.IronRaw, new ItemData
            {
                Name = "粗铁",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/milk_icon.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            _dataMap.Add(ItemType.Iron, new ItemData
            {
                Name = "铁块",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/milk_icon.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            _dataMap.Add(ItemType.CopperRaw, new ItemData
            {
                Name = "粗铜",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/milk_icon.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            _dataMap.Add(ItemType.Copper, new ItemData
            {
                Name = "铜块",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/milk_icon.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            _dataMap.Add(ItemType.SilverRaw, new ItemData
            {
                Name = "粗银",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/milk_icon.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            _dataMap.Add(ItemType.Silver, new ItemData
            {
                Name = "银块",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/milk_icon.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            _dataMap.Add(ItemType.Gold, new ItemData
            {
                Name = "黄金",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/milk_icon.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });

            _dataMap.Add(ItemType.Arrow, new ItemData
            {
                Name = "箭",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/Arrow.tres",
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 4),
                    (ItemType.Rope, 5),
                    (ItemType.Silk, 5),
                },
            });
            #endregion

            #region 法术, 发射物
            _dataMap.Add(ItemType.Fireball, new ItemData
            {
                Name = "火球符箓",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/Fireball.tres",
                CanAim = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 4),
                    (ItemType.Rope, 5),
                    (ItemType.Silk, 5),
                },
            });
            #endregion

            #region 消耗品CanConsume
            _dataMap.Add(ItemType.Banana, new ItemData
            {
                Name = "蕉",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/Banana.tres",
                CanConsume = true,
                MaxQiBonus = 5,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {

                },
            });
            #endregion

            #region 武器
            _dataMap.Add(ItemType.WoodSword, new ItemData
            {
                Name = "木剑",
                IconPath = "res://Assets/AtlasTextures/WoodSword.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 3),
                    (ItemType.Rope, 5),
                },
                CanAim = true,
            });
            _dataMap.Add(ItemType.IronSword, new ItemData
            {
                Name = "铁剑",
                IconPath = "res://Assets/AtlasTextures/IronSword.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Iron, 5),
                    (ItemType.Wood, 2),
                },
                CanAim = true,
            });
            _dataMap.Add(ItemType.GoldSword, new ItemData
            {
                Name = "金剑",
                IconPath = "res://Assets/AtlasTextures/GoldSword.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 5),
                    (ItemType.Iron, 2),
                },
                CanAim = true,
            });
            _dataMap.Add(ItemType.JadeSword, new ItemData
            {
                Name = "玉剑",
                IconPath = "res://Assets/AtlasTextures/JadeSword.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 6),
                    (ItemType.Iron, 3),
                },
                CanAim = true,
            });


            _dataMap.Add(ItemType.WoodBow, new ItemData
            {
                Name = "木弓",
                IconPath = "res://Assets/AtlasTextures/WoodBow.tres",
                MaxDur = 100,
                CanAim = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 4),
                    (ItemType.Rope, 5),
                },
            });
            _dataMap.Add(ItemType.IronBow, new ItemData
            {
                Name = "铁弓",
                IconPath = "res://Assets/AtlasTextures/IronBow.tres",
                MaxDur = 100,
                CanAim = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Iron, 4),
                    (ItemType.Rope, 3),
                },
            });
            _dataMap.Add(ItemType.GoldBow, new ItemData
            {
                Name = "金弓",
                IconPath = "res://Assets/AtlasTextures/GoldBow.tres",
                MaxDur = 100,
                CanAim = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 4),
                    (ItemType.Rope, 3),
                },
            });
            _dataMap.Add(ItemType.JadeBow, new ItemData
            {
                Name = "玉弓",
                IconPath = "res://Assets/AtlasTextures/JadeBow.tres",
                MaxDur = 100,
                CanAim = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 5),
                    (ItemType.Rope, 3),
                    (ItemType.Silk, 3),
                },
            });
            #endregion

            #region 工具
            _dataMap.Add(ItemType.WoodPot, new ItemData
            {
                Name = "木壶",
                IconPath = "res://Assets/AtlasTextures/WoodPot.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 4),
                    (ItemType.Rope, 5),
                },
            });
            _dataMap.Add(ItemType.WoodRod, new ItemData
            {
                Name = "木竿",
                IconPath = "res://Assets/AtlasTextures/WoodRod.tres",
                MaxDur = 100,
                CanAim = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 3),
                    (ItemType.Rope, 5),
                },
            });

            _dataMap.Add(ItemType.GoldPot, new ItemData
            {
                Name = "金壶",
                IconPath = "res://Assets/AtlasTextures/GoldPot.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 3),
                },
            });
            _dataMap.Add(ItemType.GoldRod, new ItemData
            {
                Name = "金竿",
                IconPath = "res://Assets/AtlasTextures/GoldRod.tres",
                MaxDur = 100,
                CanAim = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 3),
                    (ItemType.Rope, 3),
                },
            });

            _dataMap.Add(ItemType.JadePot, new ItemData
            {
                Name = "玉壶",
                IconPath = "res://Assets/AtlasTextures/JadePot.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 5),
                },
            });
            _dataMap.Add(ItemType.JadeRod, new ItemData
            {
                Name = "玉竿",
                IconPath = "res://Assets/AtlasTextures/JadeRod.tres",
                MaxDur = 100,
                CanAim = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 4),
                    (ItemType.Rope, 3),
                },
            });
            #endregion

            #region 法宝
            _dataMap.Add(ItemType.WoodHelmet, new ItemData
            {
                Name = "木盔",
                IconPath = "res://Assets/AtlasTextures/WoodHelmet.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Helmet,
                AtkBonus = 1,
                DefBonus = 2,
                MaxHpBonus = 3,
                MaxQiBonus = 4,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 4),
                    (ItemType.Rope, 5),
                    (ItemType.Silk, 5),
                },
            });
            _dataMap.Add(ItemType.WoodArmor, new ItemData
            {
                Name = "木甲",
                IconPath = "res://Assets/AtlasTextures/WoodArmor.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Armor,
                DefBonus = 4,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 3),
                    (ItemType.Rope, 5),
                    (ItemType.Silk, 5),
                },
            });
            _dataMap.Add(ItemType.WoodBoot, new ItemData
            {
                Name = "木鞋",
                IconPath = "res://Assets/AtlasTextures/WoodBoot.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Boot,
                DefBonus = 1,
                MoveSpeedBonus = 10,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 4),
                    (ItemType.Rope, 5),
                    (ItemType.Silk, 5),
                },
            });

            _dataMap.Add(ItemType.IronHelmet, new ItemData
            {
                Name = "铁盔",
                IconPath = "res://Assets/AtlasTextures/IronHelmet.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Helmet,
                DefBonus = 5,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Iron, 5),
                    (ItemType.Silk, 3),
                },
            });
            _dataMap.Add(ItemType.IronArmor, new ItemData
            {
                Name = "铁甲",
                IconPath = "res://Assets/AtlasTextures/IronArmor.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Armor,
                DefBonus = 8,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Iron, 8),
                    (ItemType.Silk, 5),
                },
            });
            _dataMap.Add(ItemType.IronBoot, new ItemData
            {
                Name = "铁鞋",
                IconPath = "res://Assets/AtlasTextures/IronBoot.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Boot,
                DefBonus = 3,
                MoveSpeedBonus = 20,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Iron, 4),
                    (ItemType.Silk, 2),
                },
            });

            _dataMap.Add(ItemType.GoldHelmet, new ItemData
            {
                Name = "金盔",
                IconPath = "res://Assets/AtlasTextures/GoldHelmet.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Helmet,
                DefBonus = 8,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 5),
                    (ItemType.Silk, 3),
                },
            });
            _dataMap.Add(ItemType.GoldArmor, new ItemData
            {
                Name = "金甲",
                IconPath = "res://Assets/AtlasTextures/GoldArmor.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Armor,
                DefBonus = 12,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 8),
                    (ItemType.Silk, 5),
                },
            });
            _dataMap.Add(ItemType.GoldBoot, new ItemData
            {
                Name = "金鞋",
                IconPath = "res://Assets/AtlasTextures/GoldBoot.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Boot,
                DefBonus = 4,
                MoveSpeedBonus = 30,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 4),
                    (ItemType.Silk, 2),
                },
            });

            _dataMap.Add(ItemType.JadeHelmet, new ItemData
            {
                Name = "玉盔",
                IconPath = "res://Assets/AtlasTextures/JadeHelmet.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Helmet,
                DefBonus = 14,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 6),
                    (ItemType.Silk, 5),
                },
            });
            _dataMap.Add(ItemType.JadeArmor, new ItemData
            {
                Name = "玉甲",
                IconPath = "res://Assets/AtlasTextures/JadeArmor.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Armor,
                DefBonus = 20,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 10),
                    (ItemType.Silk, 8),
                },
            });
            _dataMap.Add(ItemType.JadeBoot, new ItemData
            {
                Name = "玉鞋",
                IconPath = "res://Assets/AtlasTextures/JadeBoot.tres",
                MaxDur = 100,
                IsArmor = true,
                ArmorSlot = ArmorSlotType.Boot,
                DefBonus = 8,
                MoveSpeedBonus = 50,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 5),
                    (ItemType.Silk, 3),
                },
            });
            #endregion


            //木制套装
            _dataMap.Add(ItemType.WoodPickaxe, new ItemData
            {
                Name = "木镐",
                IconPath = "res://Assets/AtlasTextures/WoodPickaxe.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 4),
                    (ItemType.Rope, 5),
                },
            });
            _dataMap.Add(ItemType.WoodAxe, new ItemData
            {
                Name = "木斧",
                IconPath = "res://Assets/AtlasTextures/WoodAxe.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 3),
                    (ItemType.Rope, 5),
                },
                CanAim = true,
            });

            _dataMap.Add(ItemType.IronPot, new ItemData
            {
                Name = "铁壶",
                IconPath = "res://Assets/AtlasTextures/IronPot.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Iron, 3),
                },
            });
            _dataMap.Add(ItemType.IronRod, new ItemData
            {
                Name = "铁竿",
                IconPath = "res://Assets/AtlasTextures/IronRod.tres",
                MaxDur = 100,
                CanAim = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Iron, 3),
                    (ItemType.Rope, 3),
                },
            });

            //铁制套装

            _dataMap.Add(ItemType.IronPickaxe, new ItemData
            {
                Name = "铁镐",
                IconPath = "res://Assets/AtlasTextures/IronPickaxe.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Iron, 5),
                    (ItemType.Wood, 3),
                },
            });
            _dataMap.Add(ItemType.IronAxe, new ItemData
            {
                Name = "铁斧",
                IconPath = "res://Assets/AtlasTextures/IronAxe.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Iron, 4),
                    (ItemType.Wood, 2),
                },
            });



            //金制装备

            _dataMap.Add(ItemType.GoldPickaxe, new ItemData
            {
                Name = "金镐",
                IconPath = "res://Assets/AtlasTextures/GoldPickaxe.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 5),
                    (ItemType.Wood, 3),
                },
            });
            _dataMap.Add(ItemType.GoldAxe, new ItemData
            {
                Name = "金斧",
                IconPath = "res://Assets/AtlasTextures/GoldAxe.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 4),
                    (ItemType.Wood, 2),
                },
            });



            //玉制套装

            _dataMap.Add(ItemType.JadePickaxe, new ItemData
            {
                Name = "玉镐",
                IconPath = "res://Assets/AtlasTextures/JadePickaxe.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 6),
                    (ItemType.Iron, 3),
                },
            });
            _dataMap.Add(ItemType.JadeAxe, new ItemData
            {
                Name = "玉斧",
                IconPath = "res://Assets/AtlasTextures/JadeAxe.tres",
                MaxDur = 100,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Gold, 5),
                    (ItemType.Iron, 2),
                },
            });



            #region 建筑CanBuild
            _dataMap.Add(ItemType.MainBase, new ItemData
            {
                Name = "聚灵之源",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/MainBase.tres",
                CanBuild = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.MainBaseStone, 3),
                    (ItemType.Wood, 15),
                    (ItemType.Stone, 10),
                },
            });
            _dataMap.Add(ItemType.Flag, new ItemData
            {
                Name = "法旗",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/Flag.tres",
                CanBuild = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 5),
                    (ItemType.Silk, 5),
                },
            });
            _dataMap.Add(ItemType.BuildingCraft, new ItemData
            {
                Name = "天工殿",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/BuildingCraft.tres",
                CanBuild = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 15),
                    (ItemType.Stone, 10),
                },
            });
            _dataMap.Add(ItemType.ToolCraft, new ItemData
            {
                Name = "锻器台",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/ToolCraft.tres",
                CanBuild = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Iron, 5),
                    (ItemType.Wood, 10),
                    (ItemType.Stone, 5),
                },
            });
            _dataMap.Add(ItemType.ArmorCraft, new ItemData
            {
                Name = "织锦阁",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/ArmorCraft.tres",
                CanBuild = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Silk, 10),
                    (ItemType.Wood, 8),
                },
            });
            _dataMap.Add(ItemType.MagicCraft, new ItemData
            {
                Name = "符箓台",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/MainBase.tres",
                CanBuild = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 10),
                    (ItemType.Silk, 5),
                    (ItemType.Stone, 5),
                },
            });
            _dataMap.Add(ItemType.ItemBox, new ItemData
            {
                Name = "乾坤箱",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/MainBase.tres",
                CanBuild = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 8),
                    (ItemType.Stone, 8),
                    (ItemType.Iron, 2),
                },
            });
            _dataMap.Add(ItemType.TreeGrow, new ItemData
            {
                Name = "树苗",
                MaxCount = 99,
                IconPath = "res://Assets/AtlasTextures/TreeGrow.tres",
                CanBuild = true,
                CraftRequiredItemList = new List<(ItemType, int)>()
                {
                    (ItemType.Wood, 4),
                    (ItemType.Rope, 5),
                    (ItemType.Silk, 5),
                },
            });
            #endregion
        }

        public ItemData GetData(ItemType type)
        {
            return _dataMap[type];
        }
    }
}
