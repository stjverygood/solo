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
