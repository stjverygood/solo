using Solo.Scripts.Global;

namespace Solo.Scripts.System.ItemSystem
{
    public class ItemData
    {
        public ItemType Type { get; set; }//物品种类
        public byte MaxCount { get; set; }//堆叠上限
        public string IconPath { get; set; }//图片路径
        public bool IsTool { get; set; }//是否是工具类(有耐久度)
        public int MaxDur { get; set; }//最大耐久度
    }
}
