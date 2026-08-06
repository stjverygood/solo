using Solo.Scripts.Entities.Components.AtkComponents;
using Solo.Scripts.Entities.Components.DefComponents;
using Solo.Scripts.Entities.Components.DropItemComponents;
using Solo.Scripts.Entities.Components.ExpComponents;
using Solo.Scripts.Entities.Components.HpComponents;
using Solo.Scripts.Entities.Components.InventoryComponents;
using Solo.Scripts.Entities.Components.PositionComponents;
using Solo.Scripts.Entities.Components.QiComponents;
using Solo.Scripts.Entities.Components.RealmComponents;
using Solo.Scripts.Entities.Components.StartPositionComponent;
using Solo.Scripts.Global;
using Solo.Scripts.System.InventorySystem;
using System;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Core
{
    public class EntityDataManager
    {
        private static EntityDataManager? _instance;
        public static EntityDataManager Instance => _instance ??= new EntityDataManager();
        private Dictionary<EntityType, EntityData> _dataMap = new Dictionary<EntityType, EntityData>();

        private EntityDataManager()
        {
            //_dataMap[EntityType.Player] = new PlayerData();
            List<InventorySlot> fastBarInventorySlotList = new List<InventorySlot>(4);
            for (int i = 0; i < 4; i++)
                fastBarInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Common });

            List<InventorySlot> bagInventorySlotList = new List<InventorySlot>(16);
            for (int i = 0; i < 16; i++)
                bagInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Common });

            List<InventorySlot> equipmentInventorySlotList = new List<InventorySlot>()
            {
                new InventorySlot() { Type = InventorySlotType.Helmet },
                new InventorySlot() { Type = InventorySlotType.Armor },
                new InventorySlot() { Type = InventorySlotType.Boot }
            };

            float baseRealmMaxHpBonus = 100;
            float baseRealmMaxQiBonus = 50;
            float baseRealmMaxExp = 200;
            float baseRealmAtkBonus = 9;
            float baseRealmDefBonus = 8;
            float realmMinorRate = 1.2f;//小境界提升基数
            float realmMajorRate = 5.0f;//大境界提升基数
            (RealmType type, string name, int majorLevel, int minorLevel)[] realmConfigList =
            [
                (RealmType.LianQi1, "炼气期一层", 1, 1),
                (RealmType.LianQi2, "炼气期二层", 1, 2),
                (RealmType.LianQi3, "炼气期三层", 1, 3),
                (RealmType.LianQi4, "炼气期四层", 1, 4),
                (RealmType.LianQi5, "炼气期五层", 1, 5),

                (RealmType.ZhuJi1, "筑基期初期", 2, 1),
                (RealmType.ZhuJi2, "筑基期中期", 2, 2),
                (RealmType.ZhuJi3, "筑基期后期", 2, 3),

                (RealmType.JieDan1, "结丹期初期", 3, 1),
                (RealmType.JieDan2, "结丹期中期", 3, 2),
                (RealmType.JieDan3, "结丹期后期", 3, 3),

                (RealmType.YuanYin1, "元婴期初期", 4, 1),
                (RealmType.YuanYin2, "元婴期中期", 4, 2),
                (RealmType.YuanYin3, "元婴期后期", 4, 3),

                (RealmType.HuaShen, "化神期", 6, 1),
            ];
            Dictionary<RealmType, RealmData> playerRealmDataMap = new Dictionary<RealmType, RealmData>();
            foreach ((RealmType type, string name, int majorLevel, int minorLevel) config in realmConfigList)
            {
                float multiplier = MathF.Pow(realmMajorRate, config.majorLevel) * MathF.Pow(realmMinorRate, config.minorLevel);
                playerRealmDataMap.Add(config.type, new RealmData()
                {
                    Name = config.name,
                    MaxHpBonus = baseRealmMaxHpBonus * multiplier,
                    MaxQiBonus = baseRealmMaxQiBonus * multiplier,
                    MaxExp = baseRealmMaxExp * multiplier,
                    AtkBonus = baseRealmAtkBonus * multiplier,
                    DefBonus = baseRealmDefBonus * multiplier,
                });
            }


            _dataMap[EntityType.Player] = new EntityData()
            {
                ComponentDataMap = new Dictionary<Type, ComponentData?>()
                {
                    { typeof(RealmComponent), new RealmComponentData() { RealmDataMap = playerRealmDataMap } },
                    { typeof(HpComponent), new HpComponentData() { BaseMaxHp = 100 } },
                    { typeof(QiComponent), new QiComponentData() { BaseMaxQi = 100 } },
                    { typeof(ExpComponent), null },
                    { typeof(AtkComponent), new AtkComponentData() { BaseAtk = 100 } },
                    { typeof(DefComponent), new DefComponentData() { BaseDef = 100 } },
                    { typeof(InventoryComponent), new InventoryComponentData() {FastBarInventorySlotList = fastBarInventorySlotList,BagInventorySlotList = bagInventorySlotList, EquipmentInventorySlotList = equipmentInventorySlotList,}},
                    { typeof(PositionComponent), null},
                    { typeof(StartPositionComponent), null}
                }
            };

            //_dataMap[EntityType.NormalMeleeEnemy] = new EnemyData()
            //{
            //    MaxHp = 100,
            //    Atk = 40,
            //    Def = 30,
            //    MoveSpeed = 30,
            //    ViewRange = 100,
            //    ViewRangeSq = 100 * 100,
            //    AtkRange = 20,
            //    AtkRangeSq = 20 * 20,
            //    IdleDuration = 1,
            //    PatrolRange = 200,
            //    ProjectileType = ProjectileType.SwordWave,
            //    DropItemDropInfoList = new List<DropItemDropInfo>()
            //    {
            //        new DropItemDropInfo(){ Type = ItemType.Stone, Times = 1, Chance = 1f},
            //    },
            //    ExpBallDropInfo = new ExpBallDropInfo() { MinExp = 100, MaxExp = 300, Times = 5, Chance = 0.8f }
            //};
            //_dataMap[EntityType.SpeedMeleeEnemy] = new EnemyData()
            //{
            //    MaxHp = 10,
            //    Atk = 10,
            //    Def = 10,
            //    MoveSpeed = 50,
            //    ViewRange = 100,
            //    ViewRangeSq = 100 * 100,
            //    AtkRange = 20,
            //    AtkRangeSq = 20 * 20,
            //    IdleDuration = 0.1f,
            //    PatrolRange = 200,
            //    ProjectileType = ProjectileType.SwordWave,
            //    DropItemDropInfoList = new List<DropItemDropInfo>()
            //    {
            //    },
            //    ExpBallDropInfo = new ExpBallDropInfo() { MinExp = 100, MaxExp = 300, Times = 5, Chance = 0.8f }
            //};
            //_dataMap[EntityType.StrongMeleeEnemy] = new EnemyData()
            //{
            //    MaxHp = 1000,
            //    Atk = 500,
            //    Def = 300,
            //    MoveSpeed = 10,
            //    ViewRange = 100,
            //    ViewRangeSq = 100 * 100,
            //    AtkRange = 20,
            //    AtkRangeSq = 20 * 20,
            //    IdleDuration = 3,
            //    PatrolRange = 200,
            //    ProjectileType = ProjectileType.SwordWave,
            //    DropItemDropInfoList = new List<DropItemDropInfo>()
            //    {
            //    },
            //    ExpBallDropInfo = new ExpBallDropInfo() { MinExp = 100, MaxExp = 300, Times = 5, Chance = 0.8f }
            //};
            //_dataMap[EntityType.NormalRangedEnemy] = new EnemyData()
            //{
            //    MaxHp = 100,
            //    Atk = 40,
            //    Def = 30,
            //    MoveSpeed = 30,
            //    ViewRange = 100,
            //    ViewRangeSq = 100 * 100,
            //    AtkRange = 200,
            //    AtkRangeSq = 200 * 200,
            //    IdleDuration = 1,
            //    PatrolRange = 200,
            //    ProjectileType = ProjectileType.Arrow,
            //    DropItemDropInfoList = new List<DropItemDropInfo>()
            //    {
            //    },
            //    ExpBallDropInfo = new ExpBallDropInfo() { MinExp = 100, MaxExp = 300, Times = 5, Chance = 0.8f }
            //};

            _dataMap[EntityType.Tree] = new EntityData()
            {
                ComponentDataMap = new Dictionary<Type, ComponentData?>()
                {
                    { typeof(PositionComponent), null },
                    { typeof(HpComponent), new HpComponentData() { BaseMaxHp = 100 } },
                    { typeof(DefComponent), new DefComponentData() { BaseDef = 100 } },
                    { typeof(DropItemComponent), new DropItemComponentData()
                        {
                            DropInfoList = new List<DropItemDropInfo>()
                            {
                                new DropItemDropInfo() { Type = ItemType.Wood, Times = 4, Chance = 0.7f},
                                new DropItemDropInfo() { Type = ItemType.Stone, Times = 2, Chance = 0.6f}
                            }
                        }
                    }
                }
            };
            _dataMap[EntityType.Grass] = new EntityData()
            {
                ComponentDataMap = new Dictionary<Type, ComponentData?>()
                {
                    { typeof(PositionComponent), null },
                    { typeof(HpComponent), new HpComponentData() { BaseMaxHp = 100 } },
                    { typeof(DefComponent), new DefComponentData() { BaseDef = 100 } },
                    { typeof(DropItemComponent), new DropItemComponentData()
                        {
                            DropInfoList = new List<DropItemDropInfo>()
                            {
                                new DropItemDropInfo() { Type = ItemType.Wood, Times = 4, Chance = 0.7f},
                                new DropItemDropInfo() { Type = ItemType.Stone, Times = 2, Chance = 0.6f}
                            }
                        }
                    }
                }
            };
            _dataMap[EntityType.Stone] = new EntityData()
            {
                ComponentDataMap = new Dictionary<Type, ComponentData?>()
                {
                    { typeof(PositionComponent), null },
                    { typeof(HpComponent), new HpComponentData() { BaseMaxHp = 100 } },
                    { typeof(DefComponent), new DefComponentData() { BaseDef = 100 } },
                    { typeof(DropItemComponent), new DropItemComponentData()
                        {
                            DropInfoList = new List<DropItemDropInfo>()
                            {
                                new DropItemDropInfo() { Type = ItemType.Wood, Times = 4, Chance = 0.7f},
                                new DropItemDropInfo() { Type = ItemType.Stone, Times = 2, Chance = 0.6f}
                            }
                        }
                    }
                }
            };
            _dataMap[EntityType.Ore] = new EntityData()
            {
                ComponentDataMap = new Dictionary<Type, ComponentData?>()
                {
                    { typeof(PositionComponent), null },
                    { typeof(HpComponent), new HpComponentData() { BaseMaxHp = 100 } },
                    { typeof(DefComponent), new DefComponentData() { BaseDef = 100 } },
                    { typeof(DropItemComponent), new DropItemComponentData()
                        {
                            DropInfoList = new List<DropItemDropInfo>()
                            {
                                new DropItemDropInfo() { Type = ItemType.Wood, Times = 4, Chance = 0.7f},
                                new DropItemDropInfo() { Type = ItemType.Stone, Times = 2, Chance = 0.6f}
                            }
                        }
                    }
                }
            };
            _dataMap[EntityType.DropItem] = new EntityData()
            {
                ComponentDataMap = new Dictionary<Type, ComponentData?>()
                {
                    { typeof(PositionComponent), null },
                }
            };
            //_dataMap[EntityType.Tree] = new ResourceData()
            //{
            //    MaxHp = 500,
            //    DropInfoList = new List<DropItems.DropItemDropInfo>()
            //    {
            //        new DropItemDropInfo() { Type = ItemType.Wood, Times = 4, Chance = 0.7f},
            //        new DropItemDropInfo() { Type = ItemType.Stone, Times = 2, Chance = 0.6f}
            //    }
            //};

            //_dataMap[EntityType.Grass] = new ResourceData()
            //{
            //    MaxHp = 30,
            //    DropInfoList = new List<DropItems.DropItemDropInfo>()
            //    {
            //    }
            //};
            //_dataMap[EntityType.Stone] = new ResourceData()
            //{
            //    MaxHp = 1000,
            //    DropInfoList = new List<DropItems.DropItemDropInfo>()
            //    {
            //    }
            //};
            //_dataMap[EntityType.Ore] = new ResourceData()
            //{
            //    MaxHp = 10000,
            //    DropInfoList = new List<DropItems.DropItemDropInfo>()
            //    {
            //    }
            //};
        }

        public EntityData GetData(EntityType type)
        {
            return _dataMap[type];
        }
    }
}
