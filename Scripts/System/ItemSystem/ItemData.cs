using Solo.Scripts.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solo.Scripts.System.ItemSystem
{
    public class ItemData
    {
        public ItemType Type { get; set; }//物品种类
        public byte MaxCount { get; set; }//堆叠上限

    }
}
