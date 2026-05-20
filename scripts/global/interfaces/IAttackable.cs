using Godot;

namespace Solo.Global.Interfaces
{
    public interface IAttackable
    {
        public void TakeDamage(int damage);
        public Vector2 GetPositon();
    }
}
