using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.DropItems;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Resources
{
    public class ResourceData : EntityData
    {
        public float MaxHp;
        public required List<DropInfo> DropInfoList;//掉落物类型, 抽奖次数, 一次抽奖的概率, 
        //todo : res和enemy都要有这个表, 掉落时根据这个表传去gameManager, 在那边统一生成掉落物
        //玩家, boss这种有装备系统的, 就拿InventoryComponent来生成DropItemList, 再传去gameManager掉落
        //让dropItem自己一开始随机爆开
    }
}
