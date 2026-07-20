using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;

namespace Solo.Scripts.UI.CraftViews
{
    public partial class RequiredItemView : PanelContainer
    {
        [Export] private TextureRect _iconTr = null!;
        [Export] private Label _nameLb = null!;
        [Export] private Label _countLb = null!;


        public void Init(ItemType type, int count)
        {
            ItemData data = ItemDataManager.Instance.GetData(type);
            _iconTr.Texture = GD.Load<Texture2D>(data.IconPath);
            _nameLb.Text = data.Name;
            _countLb.Text = $"*{count}";
        }

        public void SetIsEnough(bool isEnough)
        {
            if (isEnough)
            {
                _nameLb.Modulate = Color.Color8(62, 137, 72);//99, 199, 77
                _countLb.Modulate = Color.Color8(62, 137, 72);
            }
            else
            {
                _nameLb.Modulate = Color.Color8(228, 59, 68);//162, 38, 51
                _countLb.Modulate = Color.Color8(228, 59, 68);
            }
        }
    }
}
