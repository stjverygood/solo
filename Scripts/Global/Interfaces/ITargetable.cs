using Godot;

namespace Solo.Scripts.Global.Interfaces
{
    public interface ITargetable
    {
        public bool CanInteract();
        public bool CanAtk();
        public void ShowInteractTip(bool isShow);
        public void ShowAtkTip(bool isShow);
        public Vector2 GetWorldPosition();
        public void TakeDamage(float damage, ItemType? itemType);
        public void ShowOutline(bool isShow);
    }
}
