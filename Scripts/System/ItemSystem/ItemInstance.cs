using Solo.Scripts.Global;

namespace Solo.Scripts.System.ItemSystem
{
    public class ItemInstance
    {
        public ItemType Type { get; set; }
        public int Count { get; set; }
        public int CurDur { get; set; }//当前耐久度(若是工具才有意义)
        //后期加上附魔, 重命名, 各种独特效果...
    }
}
