using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Global;
using Solo.Scripts.UI.HUDs;
using Solo.Scripts.UI.InteractViews;
using Solo.Scripts.UI.InventoryViews;
using Solo.Scripts.UI.ScreenViews;

public partial class UIManager : CanvasLayer
{

    [Export] public FastAttributeView FastAttributeView = null!;
    [Export] public FastBarView FastBarView = null!;
    [Export] public InventoryManagerView InventoryManagerView = null!;
    [Export] public Control InteractControl = null!;
    [Export] public CharacterView CharacterView = null!;
    [Export] public PauseView PauseView = null!;

    public void Init()
    {
        GameManager.Instance.UIManager = this;
        PauseView.Visible = false;
        ProcessMode = ProcessModeEnum.Always;

        var playerCore = GameManager.Instance.Player.Core;
        FastAttributeView.Init(playerCore.GetComponent<RealmComponent>(), playerCore.GetComponent<HpComponent>(), playerCore.GetComponent<QiComponent>(), playerCore.GetComponent<ExpComponent>());
        FastBarView.Init(playerCore.GetComponent<InventoryComponent>().FastBarInventory, GameManager.Instance.Player.CurFastBarIndex);
        InventoryManagerView.Init(playerCore.GetComponent<InventoryComponent>());
        CharacterView.Init();
    }

    public void PauseGame()
    {

    }
}
