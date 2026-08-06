using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.System.CraftSystem
{
    public class CraftDataManager
    {
        private static CraftDataManager _instance;
        public static CraftDataManager Instance => _instance ??= new CraftDataManager();
        private Dictionary<CraftType, CraftData> _craftDataMap = new Dictionary<CraftType, CraftData>();
        private CraftDataManager()
        {
            _craftDataMap.Add(CraftType.Fast, new CraftData()
            {
                Name = "便携造物台",
                ItemList = new List<ItemType>()
                {
                    ItemType.MainBase,
                    ItemType.BuildingCraft,
                    ItemType.Rope,
                    ItemType.WoodSword,
                    ItemType.WoodBow,
                    ItemType.WoodPickaxe,
                    ItemType.WoodAxe,
                    ItemType.WoodPot,
                    ItemType.WoodRod,
                    ItemType.WoodHelmet,
                    ItemType.WoodArmor,
                    ItemType.WoodBoot,
                    ItemType.Arrow,
                    ItemType.Fireball,
                    ItemType.TreeGrow,

                    ItemType.Grass,//草
                    ItemType.Stone,//石头

                    ItemType.MonsterCore1,
                    ItemType.MonsterCore2,
                    ItemType.MonsterCore3,
                    ItemType.MonsterCore4,

                    ItemType.Leather1,
                    ItemType.Leather2,
                    ItemType.Leather3,
                    ItemType.Leather4,

                    ItemType.Wood1,
                    ItemType.Wood2,
                    ItemType.Wood3,
                    ItemType.Wood4,

                    ItemType.Metal1,
                    ItemType.Metal2,
                    ItemType.Metal3,
                    ItemType.Metal4,

                    ItemType.QiStone1,
                    ItemType.QiStone2,
                    ItemType.QiStone3,
                    ItemType.QiStone4,

                    ItemType.Sword1,
                    ItemType.Sword2,
                    ItemType.Sword3,
                    ItemType.Sword4,

                    ItemType.Bow1,
                    ItemType.Bow2,
                    ItemType.Bow3,
                    ItemType.Bow4,
                },
                CraftBtnText = "合成",
            });
            _craftDataMap.Add(CraftType.Building, new CraftData()
            {
                Name = "天工殿",
                ItemList = new List<ItemType>()
                {
                    ItemType.Flag,
                    ItemType.ToolCraft,
                    ItemType.ArmorCraft,
                },
                CraftBtnText = "合成",
            });
            _craftDataMap.Add(CraftType.Weapon, new CraftData()
            {
                Name = "兵器库",
                ItemList = new List<ItemType>()
                {
                    ItemType.IronSword,
                    ItemType.IronBow,
                    ItemType.GoldSword,
                    ItemType.GoldBow,
                    ItemType.JadeSword,
                    ItemType.JadeBow,
                },
                CraftBtnText = "合成",
            });
            _craftDataMap.Add(CraftType.Tool, new CraftData()
            {
                Name = "工具合成台",
                ItemList = new List<ItemType>()
                {
                    ItemType.IronPickaxe,
                    ItemType.IronAxe,
                    ItemType.IronPot,
                    ItemType.IronRod,
                    ItemType.GoldPickaxe,
                    ItemType.GoldAxe,
                    ItemType.GoldPot,
                    ItemType.GoldRod,
                    ItemType.JadePickaxe,
                    ItemType.JadeAxe,
                    ItemType.JadePot,
                    ItemType.JadeRod,
                },
                CraftBtnText = "合成",
            });
            _craftDataMap.Add(CraftType.Armor, new CraftData()
            {
                Name = "防具合成台",
                ItemList = new List<ItemType>()
                {
                    ItemType.IronHelmet,
                    ItemType.IronArmor,
                    ItemType.IronBoot,
                    ItemType.GoldHelmet,
                    ItemType.GoldArmor,
                    ItemType.GoldBoot,
                    ItemType.JadeHelmet,
                    ItemType.JadeArmor,
                    ItemType.JadeBoot,
                },
                CraftBtnText = "合成",
            });
        }

        public CraftData GetData(CraftType type)
        {
            return _craftDataMap[type];
        }
    }
}
