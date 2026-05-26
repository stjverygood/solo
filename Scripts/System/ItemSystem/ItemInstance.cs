namespace Solo.Scripts.System.ItemSystem
{
    public class ItemInstance
    {
        //对于可堆叠物品, ItemInstance只是个容器, 传递时应该深拷贝
        //对于不可堆叠物品, ItemInstance是全局唯一的实例, 有独自的状态, 所以传递时只传递引用

        public ItemData Data { get; set; }
        public int CurDur { get; set; }//当前耐久度(若是工具才有意义)
        public int Count { get; set; }
        //后期加上附魔, 重命名, 各种独特效果...
    }
}
