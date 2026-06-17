using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.BuildingSystem;

public partial class BuildingPreview : Node2D
{
    public BuildingType Type;
    [Export] private Sprite2D _sprite;
    [Export] private PackedScene _buildingPs;
    private Vector2 _curDir = Vector2.Down;

    public void Init(ItemType itemType, Vector2 mousePos)
    {
        switch (itemType)
        {
            case ItemType.MainBase:
                Type = BuildingType.MainBase;
                break;
            case ItemType.Flag:
                Type = BuildingType.Flag;
                break;
            case ItemType.BuildingCraft:
                Type = BuildingType.BuildingCraft;
                break;
            case ItemType.ToolCraft:
                Type = BuildingType.ToolCraft;
                break;
            case ItemType.ArmorCraft:
                Type = BuildingType.ArmorCraft;
                break;
            case ItemType.Crucible:
                Type = BuildingType.Crucible;
                break;
            case ItemType.ItemBox:
                Type = BuildingType.ItemBox;
                break;
        }
        BuildingData buildingData = BuildingDataManager.Instance.GetBuildingData(Type);
        _sprite.Texture = GD.Load<Texture2D>(buildingData.TexturePath);
        _sprite.Position = new Vector2(0, -(buildingData.TextureHeight - buildingData.Height) * 16 / 2);
        RefreshPosition(mousePos);
    }

    public void RefreshPosition(Vector2 mousePos)
    {
        BuildingData buildingData = BuildingDataManager.Instance.GetBuildingData(Type);
        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(Type, mousePos);
        bool canPlace = GameManager.Instance.BuildingManager.CanPlaced(Type, snapPos);
        GlobalPosition = snapPos;
        _sprite.Modulate = canPlace ? new Color(0, 1, 0, 0.2f) : new Color(1, 0, 0, 0.2f);
    }

    public bool Build()
    {
        GameManager.Instance.BuildingManager.Place(Type, GlobalPosition);

        switch (Type)
        {
            case BuildingType.MainBase:
                Building bd = _buildingPs.Instantiate<Building>();
                bd.Init(Type, GlobalPosition);//没有特殊功能的, 用buildingBase脚本, 走普通初始化函数
                GetTree().CurrentScene.AddChild(bd);
                break;
        }
        return true;
    }
}
