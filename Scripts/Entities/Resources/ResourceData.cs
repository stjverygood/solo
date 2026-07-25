using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.DropItems;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Resources
{
    public class ResourceData : EntityData
    {
        public float MaxHp;
        public required List<DropItemDropInfo> DropInfoList;//掉落物类型, 抽奖次数, 一次抽奖的概率, 
    }
}
