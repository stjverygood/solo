using Godot;

namespace Solo.Scripts.Global
{
    public partial class HpBarControl : Control
    {
        [Export] public ProgressBar _hpPb;
        [Export] public Label _hpLb;

        // 用来保存当前的 Tween 引用，以便随时重置倒计时
        private Tween _hideTween;

        public void Refresh(float maxHp, float curHp)
        {
            Visible = true;
            _hpPb.MaxValue = maxHp;
            _hpPb.Value = curHp;
            _hpLb.Text = $"{curHp}";

            if (_hideTween != null && _hideTween.IsValid())
                _hideTween.Kill();
            _hideTween = CreateTween();
            _hideTween.TweenCallback(Callable.From(() => Visible = false)).SetDelay(3.0f);
        }
    }
}