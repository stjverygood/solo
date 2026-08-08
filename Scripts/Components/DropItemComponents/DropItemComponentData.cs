using Solo.Scripts.Components.Core;
using System.Collections.Generic;

namespace Solo.Scripts.Components.DropItemComponents
{
    public class DropItemComponentData : ComponentData
    {
        public required List<DropItemDropInfo> DropInfoList;//掉落物类型, 抽奖次数, 一次中奖的概率, 
    }
}
