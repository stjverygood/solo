namespace Solo.Scripts.System.SaveSystem
{
    public class PlayerSaveData
    {
        public float StartPosX { get; set; } = 0;//出生位置, 默认是(0, 0), 只能靠主基地重置
        public float StartPosY { get; set; } = 0;
        public float PosX { get; set; } = 0;//玩家上次位置
        public float PosY { get; set; } = 0;
        public float MaxHp { get; set; } = 100;//血量
        public float CurHp { get; set; } = 100;
        public float MaxHg { get; set; } = 100;//饥饿值
        public float CurHg { get; set; } = 100;

    }
}
