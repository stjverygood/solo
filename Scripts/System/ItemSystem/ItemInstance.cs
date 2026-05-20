using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solo.Scripts.System.ItemSystem
{
    public class ItemInstance
    {
        public ItemData Data;
        public int CurDur;//当前耐久度(若是工具才有意义)
        public int Count;
        //后期加上附魔, 重命名, 各种独特效果...
    }
}
