using Godot;

namespace Solo.Scripts.UI.HUDs
{
    public partial class HudView : Control
    {
        [Export] private FastBarView _fastBarView = null!;
        [Export] private FastAttributeView _fastAttributeView = null!;
    }
}

