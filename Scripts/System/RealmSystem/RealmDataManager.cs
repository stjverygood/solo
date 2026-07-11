using Solo.Scripts.Global;
using System;
using System.Collections.Generic;

namespace Solo.Scripts.System.RealmSystem
{
    public class RealmDataManager
    {
        private static RealmDataManager _instance;
        public static RealmDataManager Instance => _instance ??= new RealmDataManager();
        private Dictionary<RealmType, RealmData> _realmDataMap = new Dictionary<RealmType, RealmData>();

        private static readonly float BaseAtk = 9;
        private static readonly float BaseDef = 8;
        private static readonly float BaseMaxHp = 100;
        private static readonly float BaseMaxQi = 50;
        private static readonly float BaseMaxExp = 200;

        public float MinorRate = 1.2f;//小境界提升基数
        private static readonly float MajorRate = 5.0f;//大境界提升基数



        private RealmDataManager()
        {
            //炼气期
            _realmDataMap.Add(RealmType.LianQi1, new RealmData()
            {
                Name = "炼气期一层",
                Atk = BaseAtk * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 1),
                Def = BaseDef * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 1),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 1),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 1),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 1),
            });
            _realmDataMap.Add(RealmType.LianQi2, new RealmData()
            {
                Name = "炼气期二层",
                Atk = BaseAtk * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 2),
                Def = BaseDef * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 2),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 2),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 2),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 2),
            });
            _realmDataMap.Add(RealmType.LianQi3, new RealmData()
            {
                Name = "炼气期三层",
                Atk = BaseAtk * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 3),
                Def = BaseDef * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 3),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 3),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 3),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 3),
            });
            _realmDataMap.Add(RealmType.LianQi4, new RealmData()
            {
                Name = "炼气期四层",
                Atk = BaseAtk * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 4),
                Def = BaseDef * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 4),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 4),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 4),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 4),
            });
            _realmDataMap.Add(RealmType.LianQi5, new RealmData()
            {
                Name = "炼气期五层",
                Atk = BaseAtk * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 5),
                Def = BaseDef * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 5),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 5),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 5),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 1) * MathF.Pow(MinorRate, 5),
            });

            //筑基期
            _realmDataMap.Add(RealmType.ZhuJi1, new RealmData()
            {
                Name = "筑基期初期",
                Atk = BaseAtk * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 1),
                Def = BaseDef * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 1),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 1),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 1),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 1),
            });
            _realmDataMap.Add(RealmType.ZhuJi2, new RealmData()
            {
                Name = "筑基期中期",
                Atk = BaseAtk * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 2),
                Def = BaseDef * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 2),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 2),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 2),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 2),
            });
            _realmDataMap.Add(RealmType.ZhuJi3, new RealmData()
            {
                Name = "筑基期后期",
                Atk = BaseAtk * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 3),
                Def = BaseDef * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 3),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 3),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 3),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 2) * MathF.Pow(MinorRate, 3),
            });

            //金丹期
            _realmDataMap.Add(RealmType.JinDan1, new RealmData()
            {
                Name = "金丹期初期",
                Atk = BaseAtk * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 1),
                Def = BaseDef * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 1),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 1),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 1),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 1),
            });
            _realmDataMap.Add(RealmType.JinDan2, new RealmData()
            {
                Name = "金丹期中期",
                Atk = BaseAtk * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 2),
                Def = BaseDef * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 2),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 2),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 2),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 2),
            });
            _realmDataMap.Add(RealmType.JinDan3, new RealmData()
            {
                Name = "金丹期后期",
                Atk = BaseAtk * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 3),
                Def = BaseDef * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 3),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 3),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 3),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 3) * MathF.Pow(MinorRate, 3),
            });

            //元婴期
            _realmDataMap.Add(RealmType.YuanYin1, new RealmData()
            {
                Name = "元婴期初期",
                Atk = BaseAtk * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 1),
                Def = BaseDef * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 1),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 1),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 1),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 1),
            });
            _realmDataMap.Add(RealmType.YuanYin2, new RealmData()
            {
                Name = "元婴期中期",
                Atk = BaseAtk * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 2),
                Def = BaseDef * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 2),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 2),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 2),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 2),
            });
            _realmDataMap.Add(RealmType.YuanYin3, new RealmData()
            {
                Name = "元婴期后满",
                Atk = BaseAtk * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 3),
                Def = BaseDef * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 3),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 3),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 3),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 4) * MathF.Pow(MinorRate, 3),
            });

            //化身期
            _realmDataMap.Add(RealmType.HuaShen, new RealmData()
            {
                Name = "化神期",
                Atk = BaseAtk * MathF.Pow(MajorRate, 6) * MathF.Pow(MinorRate, 1),
                Def = BaseDef * MathF.Pow(MajorRate, 6) * MathF.Pow(MinorRate, 1),
                MaxHp = BaseMaxHp * MathF.Pow(MajorRate, 6) * MathF.Pow(MinorRate, 1),
                MaxQi = BaseMaxQi * MathF.Pow(MajorRate, 6) * MathF.Pow(MinorRate, 1),
                MaxExp = BaseMaxExp * MathF.Pow(MajorRate, 6) * MathF.Pow(MinorRate, 1),
            });
        }

        public RealmData GetData(RealmType type)
        {
            return _realmDataMap[type];
        }
    }
}
