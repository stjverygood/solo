using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;

public partial class RequiredItemView : PanelContainer
{
    [Export] private TextureRect _iconTr;
    [Export] private Label _nameLb;
    [Export] private Label _countLb;


    public void Init(ItemType type, int count)
    {
        ItemData data = ItemDataManager.Instance.GetItemData(type);
        _iconTr.Texture = GD.Load<Texture2D>(data.IconPath);
        _nameLb.Text = data.Name;
        _countLb.Text = $"*{count}";
    }

    public void SetIsEnough(bool isEnough)
    {
        if (isEnough)
        {
            _nameLb.Modulate = new Color(1, 1, 1);//162, 38, 51
            _countLb.Modulate = new Color(1, 1, 1);
        }
        else
        {
            _nameLb.Modulate = Color.Color8(162, 38, 51);//162, 38, 51
            _countLb.Modulate = Color.Color8(162, 38, 51);
        }
    }
}
