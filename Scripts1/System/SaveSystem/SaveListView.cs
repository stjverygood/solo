using Godot;
using Solo.Scripts.Global;

namespace Solo.Scripts.System.SaveSystem
{
    public partial class SaveListView : Control
    {
        [Export] private Button _backBtn;
        [Export] private Button _delSaveBtn;
        [Export] private Button _addSaveBtn;
        [Export] private Button _goBtn;
        [Export] private GridContainer _slotGc;
        [Export] private PackedScene _saveSlotViewPs;

        public override void _Ready()
        {
            _backBtn.Pressed += () =>
            {
                GameManager.Instance.ChangeGameState(GameState.StartMenu);
            };
            _delSaveBtn.Pressed += () =>
            {
                SaveManager.Instance.RemoveSave(_curSelectedSlotView.SaveInfo.Id);
                _curSelectedSlotView = null;
                RefreshSaveSlotList();
            };
            _addSaveBtn.Pressed += () =>
            {
                SaveManager.Instance.CreateSave($"修仙界{SaveManager.Instance.SaveInfoList.Count}(测试存档)");
                RefreshSaveSlotList();
            };
            _goBtn.Pressed += () =>
            {
                //GameManager.Instance.ChangeGameState(GameState.StartMenu);
            };

            RefreshSaveSlotList();
        }

        private void RefreshSaveSlotList()
        {
            foreach (Node child in _slotGc.GetChildren())
            {
                child.QueueFree();
            }
            for (int i = 0; i < SaveManager.Instance.SaveInfoList.Count; i++)
            {
                SaveSlotView slotView = _saveSlotViewPs.Instantiate<SaveSlotView>();
                slotView.Init(this, SaveManager.Instance.SaveInfoList[i]);
                _slotGc.AddChild(slotView);
            }
        }

        public override void _Process(double delta)
        {
        }

        private SaveSlotView _curSelectedSlotView;
        public void ChangeSelectedSlot(SaveSlotView newSlot)
        {
            if (_curSelectedSlotView != null)
                _curSelectedSlotView.SelectedToNormal();
            _curSelectedSlotView = newSlot;
        }
    }
}


