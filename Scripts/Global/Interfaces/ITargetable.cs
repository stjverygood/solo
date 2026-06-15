using Godot;

namespace Solo.Scripts.Global.Interfaces
{
    public interface ITargetable
    {
        public Vector2 GetWorldPosition();
        public void ShowOutline(bool isShow);
        public bool IsVaild();

        public bool CanAtk();
        public void TakeDamage(float damage, ItemType? itemType);

        public bool CanInteract();
        public void Interact();
    }
}
