using Godot;
using Solo.Scripts.Levels;
using Solo.Scripts.System.SaveSystem;

public partial class MainMenuView : Control
{

    [Export] private StartMenuView _startMenuView = null!;
    [Export] private SaveListView _saveListView = null!;

    public override void _Ready()
    {
        GD.Print("MainMenuView _Ready!");
        _startMenuView.OnStartBtnPressed += () =>
        {
            _startMenuView.Visible = false;
            _saveListView.Visible = true;

        };
        _saveListView.BackBtnPressed += () =>
        {
            _startMenuView.Visible = true;
            _saveListView.Visible = false;
        };
    }

    public void Init()
    {
        GD.Print("MainMenuView Init!");
        _startMenuView.Visible = true;
        _saveListView.Visible = false;
    }
}
