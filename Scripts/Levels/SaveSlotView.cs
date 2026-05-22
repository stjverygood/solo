using Godot;

namespace Solo.Scripts.Levels
{
    public enum SaveSlotViewState
    {
        Normal,
        Hover,
        Selected,
    }

    public partial class SaveSlotView : Control
    {
        private Tween _tween;
        [Export] private ColorRect _bgCr;
        [Export] private Control _animRoot;

        private SaveSlotViewState _curState;


        private Color _bgCrNormalColor;

        public override void _Ready()
        {
            MouseEntered += WorldSlotView_MouseEntered;
            MouseExited += WorldSlotView_MouseExited;
            _bgCrNormalColor = _bgCr.Color;
        }

        public override void _GuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (_curState == SaveSlotViewState.Hover)
                {
                    _tween?.Kill();
                    _tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
                    _tween.TweenProperty(_animRoot, "scale", new Vector2(0.9f, 0.9f), 0.1);
                    _bgCr.Color = _bgCr.Color = new Color(_bgCrNormalColor.R, _bgCrNormalColor.G, _bgCrNormalColor.B, 0.2f);
                    _curState = SaveSlotViewState.Selected;
                    return;
                }
                if (_curState == SaveSlotViewState.Selected)
                {
                    _tween?.Kill();
                    _tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
                    _tween.TweenProperty(_animRoot, "scale", new Vector2(0.95f, 0.95f), 0.1);
                    _bgCr.Color = new Color(_bgCrNormalColor.R, _bgCrNormalColor.G, _bgCrNormalColor.B, 0.8f);
                    _curState = SaveSlotViewState.Hover;
                    return;
                }
            }
        }

        private void WorldSlotView_MouseEntered()
        {
            if (_curState == SaveSlotViewState.Normal)
            {
                _tween?.Kill();
                _tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
                _tween.TweenProperty(_animRoot, "scale", new Vector2(0.95f, 0.95f), 0.1);
                _bgCr.Color = new Color(_bgCrNormalColor.R, _bgCrNormalColor.G, _bgCrNormalColor.B, 0.8f);
                _curState = SaveSlotViewState.Hover;
                return;
            }
        }

        private void WorldSlotView_MouseExited()
        {
            if (_curState == SaveSlotViewState.Hover)
            {
                _tween?.Kill();
                _tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
                _tween.TweenProperty(_animRoot, "scale", new Vector2(1f, 1f), 0.1);
                _bgCr.Color = _bgCrNormalColor;
                _curState = SaveSlotViewState.Normal;
                return;
            }
        }
    }
}

