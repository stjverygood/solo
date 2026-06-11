using Godot;

namespace Solo.Scripts.Global.Interfaces
{
    //战斗接口, 玩家, 建筑, 单位实现
    public interface ICombat
    {
        public Vector2 GetGlobalPosition();
        public void TakeDamage();
    }
}
