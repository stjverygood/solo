using Godot;
using Solo.Scripts.Global;

namespace Solo.Scripts.System.BuildingSystem.Buildings
{
    public partial class MainBase : Building
    {
        public float QiRange = 200f;//灵气范围

        public override void Init(BuildingType type, Vector2 snapPos)
        {
            base.Init(type, snapPos);
            GameManager.Instance.MainBaseList.Add(this);
        }
        //public override void _Ready()
        //{
        //}

        //public override void _Process(double delta)
        //{
        //}
        public override void Interact()
        {
            base.Interact();
            GD.Print("MainBase Interact");
            GameManager.Instance.Player.StartPoint = GlobalPosition;
            GD.Print("ResetComponent, 重置出生点成功~~");
        }
        protected override void Die()
        {
            base.Die();
            GameManager.Instance.MainBaseList.Remove(this);
        }
    }
}
