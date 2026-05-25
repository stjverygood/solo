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
        [Export] private PanelContainer _bgPc;
        [Export] private Control _animRoot;
        [Export] private Label _nameLb;
        [Export] private Label _levelLb;
        [Export] private Label _dateLb;

        private SaveSlotViewState _curState;

        private StyleBoxFlat _normalStyle;
        private StyleBoxFlat _hoverStyle;
        private StyleBoxFlat _selectedStyle;

        public void Init(SaveListView parent, SaveInfo info)
        {
            _parent = parent;
            SaveInfo = info;
            _nameLb.Text = info.Name;
            _levelLb.Text = info.PlayerLevel;
            _dateLb.Text = info.CreateDateTime.ToString();

            MouseEntered += WorldSlotView_MouseEntered;
            MouseExited += WorldSlotView_MouseExited;

            // 初始化样式
            StyleBox styleBox = _bgPc.GetThemeStylebox("panel");
            if (styleBox is StyleBoxFlat styleBoxFlat)
            {
                _normalStyle = (StyleBoxFlat)styleBoxFlat.Duplicate();
                _normalStyle.BorderColor = new Color(1, 1, 1, 0.2f);

                _hoverStyle = (StyleBoxFlat)_normalStyle.Duplicate();
                _hoverStyle.BorderColor = new Color(1, 1, 1, 0.5f);

                _selectedStyle = (StyleBoxFlat)_normalStyle.Duplicate();
                _selectedStyle.BorderColor = new Color(1, 1, 1, 1);
            }

            // 初始状态为 Normal
            ChangeState(SaveSlotViewState.Normal);
        }

        //public override void _Ready()
        //{
        //}

        public override void _GuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (_curState == SaveSlotViewState.Selected)
                {
                    ChangeState(SaveSlotViewState.Hover);
                    _parent.ChangeSelectedSaveInfo(null);
                }
                else
                {
                    ChangeState(SaveSlotViewState.Selected);
                    _parent.ChangeSelectedSaveInfo(SaveInfo);
                }
            }
        }

        private void WorldSlotView_MouseEntered()
        {
            if (_curState == SaveSlotViewState.Normal)
                ChangeState(SaveSlotViewState.Hover);
        }

        private void WorldSlotView_MouseExited()
        {
            if (_curState == SaveSlotViewState.Hover)
                ChangeState(SaveSlotViewState.Normal);
        }

        public void ChangeState(SaveSlotViewState newState)
        {
            switch (newState)
            {
                case SaveSlotViewState.Normal:
                    _bgPc.AddThemeStyleboxOverride("panel", _normalStyle);
                    break;
                case SaveSlotViewState.Hover:
                    _bgPc.AddThemeStyleboxOverride("panel", _hoverStyle);
                    break;
                case SaveSlotViewState.Selected:

                    _bgPc.AddThemeStyleboxOverride("panel", _selectedStyle);
                    break;
            }
            _curState = newState;
        }
    }
}

