using Solo.Scripts.Global;

namespace Solo.Scripts.System.ItemSystem
{
    public class ItemData
    {
        public ItemType Type { get; set; }//物品种类
        public byte MaxCount { get; set; }//堆叠上限
        public string IconPath { get; set; }//图片路径
    }
}
