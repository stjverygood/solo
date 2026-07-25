using Solo.Scripts.Entities.DropItems;
using Solo.Scripts.Entities.Enemies;
using Solo.Scripts.Entities.Expballs;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Entities.Resources;
using Solo.Scripts.Global;
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
            _dataMap[EntityType.Player] = new PlayerData();


            _dataMap[EntityType.NormalMeleeEnemy] = new EnemyData()
            {
                MaxHp = 100,
                Atk = 40,
                Def = 30,
                MoveSpeed = 30,
                ViewRange = 100,
                ViewRangeSq = 100 * 100,
                AtkRange = 20,
                AtkRangeSq = 20 * 20,
                IdleDuration = 1,
                PatrolRange = 200,
                ProjectileType = ProjectileType.SwordWave,
                DropItemDropInfoList = new List<DropItemDropInfo>()
                {
                    new DropItemDropInfo(){ Type = ItemType.Stone, Times = 1, Chance = 1f},
                },
                ExpBallDropInfo = new ExpBallDropInfo() { MinExp = 100, MaxExp = 300, Times = 5, Chance = 0.8f }
            };
            _dataMap[EntityType.SpeedMeleeEnemy] = new EnemyData()
            {
                MaxHp = 10,
                Atk = 10,
                Def = 10,
                MoveSpeed = 50,
                ViewRange = 100,
                ViewRangeSq = 100 * 100,
                AtkRange = 20,
                AtkRangeSq = 20 * 20,
                IdleDuration = 0.1f,
                PatrolRange = 200,
                ProjectileType = ProjectileType.SwordWave,
                DropItemDropInfoList = new List<DropItemDropInfo>()
                {
                },
                ExpBallDropInfo = new ExpBallDropInfo() { MinExp = 100, MaxExp = 300, Times = 5, Chance = 0.8f }
            };
            _dataMap[EntityType.StrongMeleeEnemy] = new EnemyData()
            {
                MaxHp = 1000,
                Atk = 500,
                Def = 300,
                MoveSpeed = 10,
                ViewRange = 100,
                ViewRangeSq = 100 * 100,
                AtkRange = 20,
                AtkRangeSq = 20 * 20,
                IdleDuration = 3,
                PatrolRange = 200,
                ProjectileType = ProjectileType.SwordWave,
                DropItemDropInfoList = new List<DropItemDropInfo>()
                {
                },
                ExpBallDropInfo = new ExpBallDropInfo() { MinExp = 100, MaxExp = 300, Times = 5, Chance = 0.8f }
            };
            _dataMap[EntityType.NormalRangedEnemy] = new EnemyData()
            {
                MaxHp = 100,
                Atk = 40,
                Def = 30,
                MoveSpeed = 30,
                ViewRange = 100,
                ViewRangeSq = 100 * 100,
                AtkRange = 200,
                AtkRangeSq = 200 * 200,
                IdleDuration = 1,
                PatrolRange = 200,
                ProjectileType = ProjectileType.Arrow,
                DropItemDropInfoList = new List<DropItemDropInfo>()
                {
                },
                ExpBallDropInfo = new ExpBallDropInfo() { MinExp = 100, MaxExp = 300, Times = 5, Chance = 0.8f }
            };

            _dataMap[EntityType.Tree] = new ResourceData()
            {
                MaxHp = 500,
                DropInfoList = new List<DropItems.DropItemDropInfo>()
                {
                    new DropItemDropInfo() { Type = ItemType.Wood, Times = 4, Chance = 0.7f},
                    new DropItemDropInfo() { Type = ItemType.Stone, Times = 2, Chance = 0.6f}
                }
            };

            _dataMap[EntityType.Grass] = new ResourceData()
            {
                MaxHp = 30,
                DropInfoList = new List<DropItems.DropItemDropInfo>()
                {
                }
            };
            _dataMap[EntityType.Stone] = new ResourceData()
            {
                MaxHp = 1000,
                DropInfoList = new List<DropItems.DropItemDropInfo>()
                {
                }
            };
            _dataMap[EntityType.Ore] = new ResourceData()
            {
                MaxHp = 10000,
                DropInfoList = new List<DropItems.DropItemDropInfo>()
                {
                }
            };
        }

        public EntityData GetData(EntityType type)
        {
            return _dataMap[type];
        }
    }
}
