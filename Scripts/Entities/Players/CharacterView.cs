using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.CraftSystem;

public partial class CharacterView : Control
{
    [Export] private CraftView _fastCraftView;

    public void Init()
    {
        _fastCraftView.Init(CraftType.Fast);
    }
}
