using Godot;
using Solo.Scripts.Global;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solo.Scripts.System.SaveSystem
{
    public partial class SaveListView : Control
    {
        [Export] private Button _backBtn = null!;
        [Export] private Button _delSaveBtn = null!;
        [Export] private Button _addSaveBtn = null!;
        [Export] private Button _goBtn = null!;
        [Export] private GridContainer _slotGc = null!;
        [Export] private PackedScene _saveSlotViewPs = null!;

        private List<SaveSlot> _slotViewList = new List<SaveSlot>();
        private SaveInfo _curSelectedSaveInfo;

        public event Action? BackBtnPressed;

        public override void _Ready()
        {
            _backBtn.Pressed += () =>
            {
                BackBtnPressed?.Invoke();
            };
            _delSaveBtn.Pressed += () =>
            {
                SaveManager.Instance.RemoveSave(_curSelectedSaveInfo?.Id);
                _curSelectedSaveInfo = null;
                RefreshSaveSlotList();
            };
            _addSaveBtn.Pressed += () =>
            {
                SaveManager.Instance.CreateSave($"修仙界{SaveManager.Instance.SaveInfoList.Count}(测试存档)");
                RefreshSaveSlotList();
            };
            _goBtn.Pressed += () =>
            {

                SaveManager.Instance.CurSaveInfo = _curSelectedSaveInfo;
                GameManager.Instance.EnterWorld();
                //GameManager.Instance.ChangeState(GameState.Loading);
            };
            RefreshSaveSlotList();
        }

        private void RefreshSaveSlotList()
        {
            foreach (Node child in _slotGc.GetChildren())
            {
                child.QueueFree();
            }
            _slotViewList.Clear();
            for (int i = 0; i < SaveManager.Instance.SaveInfoList.Count; i++)
            {
                SaveSlot slotView = _saveSlotViewPs.Instantiate<SaveSlot>();
                slotView.Init(this, SaveManager.Instance.SaveInfoList[i]);
                _slotGc.AddChild(slotView);
                _slotViewList.Add(slotView);
                if (_curSelectedSaveInfo != null && _curSelectedSaveInfo.Id == slotView.SaveInfo.Id)
                {
                    slotView.ChangeState(SaveSlotState.Selected);
                }
            }
            if (_curSelectedSaveInfo == null)
            {
                _delSaveBtn.Disabled = true;
                _goBtn.Disabled = true;
            }
            else
            {
                _delSaveBtn.Disabled = false;
                _goBtn.Disabled = false;
            }
        }

        public void ChangeSelectedSaveInfo(SaveInfo? newInfo)
        {
            if (_curSelectedSaveInfo != null)
            {
                SaveSlot lastSlotView = _slotViewList.First(view => view.SaveInfo.Id == _curSelectedSaveInfo.Id);
                lastSlotView.ChangeState(SaveSlotState.Normal);
            }
            _curSelectedSaveInfo = newInfo;
            if (_curSelectedSaveInfo == null)
            {
                _delSaveBtn.Disabled = true;
                _goBtn.Disabled = true;
            }
            else
            {
                _delSaveBtn.Disabled = false;
                _goBtn.Disabled = false;
            }
        }
    }
}


