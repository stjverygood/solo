//using Solo.Scripts.Global;
//using System;
//using System.Collections.Generic;

//namespace Solo.Scripts.System.RealmSystem
//{
//    public class RealmDataManager
//    {
//        private static RealmDataManager _instance;
//        public static RealmDataManager Instance => _instance ??= new RealmDataManager();
//        private Dictionary<RealmType, RealmData> _realmDataMap = new Dictionary<RealmType, RealmData>();

//        private static readonly float realmAtk = 9;
//        private static readonly float realmDef = 8;
//        private static readonly float realmMaxHp = 100;
//        private static readonly float realmMaxQi = 50;
//        private static readonly float realmMaxExp = 200;

//        public float realmMinorRate = 1.2f;//小境界提升基数
//        private static readonly float realmMajorRate = 5.0f;//大境界提升基数



//        private RealmDataManager()
//        {
//            //炼气期
//            _realmDataMap.Add(RealmType.LianQi1, new RealmData()
//            {
//                Name = "炼气期一层",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 1),
//                Def = realmDef * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 1),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 1),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 1),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 1),
//            });
//            _realmDataMap.Add(RealmType.LianQi2, new RealmData() { Name = "炼气期二层", Atk = realmAtk * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 2), Def = realmDef * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 2), MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 2), MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 2), MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 2), });
//            _realmDataMap.Add(RealmType.LianQi3, new RealmData() { Name = "炼气期三层", Atk = realmAtk * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 3), Def = realmDef * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 3), MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 3), MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 3), MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 3), });
//            _realmDataMap.Add(RealmType.LianQi4, new RealmData()
//            {
//                Name = "炼气期四层",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 4),
//                Def = realmDef * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 4),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 4),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 4),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 4),
//            });
//            _realmDataMap.Add(RealmType.LianQi5, new RealmData()
//            {
//                Name = "炼气期五层",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 5),
//                Def = realmDef * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 5),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 5),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 5),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 1) * MathF.Pow(realmMinorRate, 5),
//            });

//            //筑基期
//            _realmDataMap.Add(RealmType.ZhuJi1, new RealmData()
//            {
//                Name = "筑基期初期",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 1),
//                Def = realmDef * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 1),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 1),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 1),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 1),
//            });
//            _realmDataMap.Add(RealmType.ZhuJi2, new RealmData()
//            {
//                Name = "筑基期中期",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 2),
//                Def = realmDef * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 2),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 2),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 2),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 2),
//            });
//            _realmDataMap.Add(RealmType.ZhuJi3, new RealmData()
//            {
//                Name = "筑基期后期",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 3),
//                Def = realmDef * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 3),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 3),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 3),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 2) * MathF.Pow(realmMinorRate, 3),
//            });

//            //金丹期
//            _realmDataMap.Add(RealmType.JieDan1, new RealmData()
//            {
//                Name = "金丹期初期",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 1),
//                Def = realmDef * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 1),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 1),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 1),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 1),
//            });
//            _realmDataMap.Add(RealmType.JieDan2, new RealmData()
//            {
//                Name = "金丹期中期",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 2),
//                Def = realmDef * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 2),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 2),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 2),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 2),
//            });
//            _realmDataMap.Add(RealmType.JieDan3, new RealmData()
//            {
//                Name = "金丹期后期",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 3),
//                Def = realmDef * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 3),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 3),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 3),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 3) * MathF.Pow(realmMinorRate, 3),
//            });

//            //元婴期
//            _realmDataMap.Add(RealmType.YuanYin1, new RealmData()
//            {
//                Name = "元婴期初期",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 1),
//                Def = realmDef * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 1),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 1),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 1),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 1),
//            });
//            _realmDataMap.Add(RealmType.YuanYin2, new RealmData()
//            {
//                Name = "元婴期中期",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 2),
//                Def = realmDef * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 2),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 2),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 2),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 2),
//            });
//            _realmDataMap.Add(RealmType.YuanYin3, new RealmData()
//            {
//                Name = "元婴期后期",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 3),
//                Def = realmDef * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 3),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 3),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 3),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 4) * MathF.Pow(realmMinorRate, 3),
//            });

//            //化身期
//            _realmDataMap.Add(RealmType.HuaShen, new RealmData()
//            {
//                Name = "化神期",
//                Atk = realmAtk * MathF.Pow(realmMajorRate, 6) * MathF.Pow(realmMinorRate, 1),
//                Def = realmDef * MathF.Pow(realmMajorRate, 6) * MathF.Pow(realmMinorRate, 1),
//                MaxHp = realmMaxHp * MathF.Pow(realmMajorRate, 6) * MathF.Pow(realmMinorRate, 1),
//                MaxQi = realmMaxQi * MathF.Pow(realmMajorRate, 6) * MathF.Pow(realmMinorRate, 1),
//                MaxExp = realmMaxExp * MathF.Pow(realmMajorRate, 6) * MathF.Pow(realmMinorRate, 1),
//            });
//        }

//        public RealmData GetData(RealmType type)
//        {
//            return _realmDataMap[type];
//        }
//    }
//}
