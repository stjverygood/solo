using Godot;

namespace Solo.Scripts.System.SaveSystem
{
    public enum SaveSlotViewState
    {
        Normal,
        Hover,
        Selected,
    }

    public partial class SaveSlotView : Control
    {
        private SaveListView _parent;
        public SaveInfo SaveInfo;
        private Tween _tween;
        [Export] private ColorRect _bgCr;
        [Export] private Control _animRoot;
        [Export] private Label _nameLb;
        [Export] private Label _levelLb;
        [Export] private Label _dateLb;

        private SaveSlotViewState _curState;


        private Color _bgCrNormalColor;

        public void Init(SaveListView parent, SaveInfo info)
        {
            _parent = parent;
            SaveInfo = info;
            _nameLb.Text = info.Name;
            _levelLb.Text = info.PlayerLevel;
            _dateLb.Text = info.CreateDateTime.ToString();

            MouseEntered += WorldSlotView_MouseEntered;
            MouseExited += WorldSlotView_MouseExited;
            _bgCrNormalColor = _bgCr.Color;
        }

        //public override void _Ready()
        //{

        //}

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
                    _parent.ChangeSelectedSlot(this);
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

        public void SelectedToNormal()
        {
            if (_curState == SaveSlotViewState.Selected)
            {
                _tween?.Kill();
                _tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
                _tween.TweenProperty(_animRoot, "scale", new Vector2(1f, 1f), 0.1);
                _bgCr.Color = _bgCr.Color = _bgCrNormalColor;
                _curState = SaveSlotViewState.Normal;
                return;
            }
        }
    }
}

