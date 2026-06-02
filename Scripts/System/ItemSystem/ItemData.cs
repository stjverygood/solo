namespace Solo.Scripts.System.ItemSystem
{
    public class ItemData
    {
        public string Name;
        public byte MaxCount; //堆叠上限
        public string IconPath;//图片路径
        public int MaxDur;//最大耐久度, 为-1意味着没有耐久度
        public ItemData(string name, byte maxCount, string iconPath, int maxDur)
        {
            Name = name;
            MaxCount = maxCount;
            IconPath = iconPath;
            MaxDur = maxDur;
        }
    }
}
