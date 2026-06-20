using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.System.BuildingSystem.Buildings
{
    public partial class MainBase : Building, IQiRangeable
    {
        [Export] private ColorRect _QiRangeCr;
        private float _qiRange = 200f;//灵气范围



        public override void Init(BuildingType type, Vector2 snapPos)
        {
            base.Init(type, snapPos);
            ShowQiRange(false);
            GameManager.Instance.IQiRangeableList.Add(this);
        }

        public float GetQiRange()
        {
            return _qiRange;
        }
        public Vector2 GetWorldPos()
        {
            return GlobalPosition;
        }
        public void ShowQiRange(bool isShow)
        {
            _QiRangeCr.Visible = isShow;
        }
        private Tween _qiTween;
        public void ShowQiRangeForSeconds()
        {
            ShowQiRange(true);
            if (_qiTween != null && _qiTween.IsValid())
            {
                _qiTween.Kill();
            }
            _qiTween = CreateTween();
            _qiTween.TweenCallback(Callable.From(() => ShowQiRange(false))).SetDelay(3.0f);
        }


        public override void Interact()
        {
            base.Interact();
            GD.Print("MainBase Interact");
            GameManager.Instance.Player.StartPoint = GlobalPosition;
            GD.Print("ResetComponent, 重置出生点成功~~");
            foreach (IQiRangeable qiRangeable in GameManager.Instance.IQiRangeableList)
            {
                qiRangeable.ShowQiRangeForSeconds();
            }
        }



        protected override void Die()
        {
            base.Die();
            GameManager.Instance.IQiRangeableList.Remove(this);
        }


    }
}
