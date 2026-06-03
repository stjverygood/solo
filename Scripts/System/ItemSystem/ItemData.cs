namespace Solo.Scripts.System.ItemSystem
{
    public class ItemData
    {
        public string Name = "";
        public byte MaxCount = 1;//堆叠上限
        public string IconPath = "";//图片路径
        public int MaxDur = -1;//最大耐久度, 为-1意味着没有耐久度
        public bool isBuilding = false;
    }
}
