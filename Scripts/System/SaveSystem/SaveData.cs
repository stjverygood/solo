namespace Solo.Scripts.System.SaveSystem
{
    public class SaveData
    {
        //纯数据类, 记录一个存档的全部信息
        public float PlayerPosX { get; set; }
        public float PlayerPosY { get; set; }

        public SaveData()
        {
            PlayerPosX = 0;
            PlayerPosY = 0;
        }
    }


}
