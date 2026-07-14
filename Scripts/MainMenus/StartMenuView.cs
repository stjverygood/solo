using Godot;
using System;

namespace Solo.Scripts.Levels
{
    public partial class StartMenuView : Control
    {
        [Export] private Button _startBtn = null!;
        [Export] private Button _settingBtn = null!;
        [Export] private Button _collectionBtn = null!;
        [Export] private Button _aboutBtn = null!;
        [Export] private Button _exitBtn = null!;

        public event Action? OnStartBtnPressed;

        public override void _Ready()
        {
            _startBtn.Pressed += () =>
            {
                OnStartBtnPressed?.Invoke();
            };
            _exitBtn.Pressed += () =>
            {
                GetTree().Quit();
            };
        }

        public override void _Process(double delta)
        {
        }
    }
}


