using Solo.Scripts.Entities.Grasses;
using Solo.Scripts.Entities.MeleeEnemies;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Entities.Trees;
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


            _dataMap[EntityType.NormalMeleeEnemy] = new MeleeEnemyData()
            {
                MaxHp = 100,
                Atk = 40,
                Def = 30,
                MoveSpeed = 50,
                ViewRange = 100,
                ViewRangeSq = 100 * 100,
                AtkRange = 20,
                AtkRangeSq = 20 * 20,
                IdleDuration = 1,
                PatrolRange = 200,
            };
            _dataMap[EntityType.SpeedMeleeEnemy] = new MeleeEnemyData()
            {
                MaxHp = 10,
                Atk = 10,
                Def = 10,
                MoveSpeed = 100,
                ViewRange = 100,
                ViewRangeSq = 100 * 100,
                AtkRange = 20,
                AtkRangeSq = 20 * 20,
                IdleDuration = 0.1f,
                PatrolRange = 200,
            };
            _dataMap[EntityType.StrongMeleeEnemy] = new MeleeEnemyData()
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
            };

            _dataMap[EntityType.Tree] = new TreeData()
            {
                MaxHp = 100,
            };

            _dataMap[EntityType.Grass] = new GrassData()
            {
                MaxHp = 100,
            };


        }

        public EntityData GetData(EntityType type)
        {
            return _dataMap[type];
        }
    }
}
