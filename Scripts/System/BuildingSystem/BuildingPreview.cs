using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.BuildingSystem;

public partial class BuildingPreview : Node2D
{
    public BuildingType Type;
    [Export] private Sprite2D _sprite;
    private PackedScene _buildingBasePs;

    public void Init(ItemType itemType, string spritePath, Vector2 handPos)
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
        _sprite.Texture = GD.Load<Texture2D>(spritePath);
        _buildingBasePs = GD.Load<PackedScene>(BuildingDataManager.Instance.GetBuildingData(Type).PsPath);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(BuildingDataManager.Instance.GetBuildingData(Type), GlobalPosition);
        bool canPlace = GameManager.Instance.BuildingManager.CanPlaced(BuildingDataManager.Instance.GetBuildingData(Type), snapPos);
        _sprite.Modulate = canPlace ? new Color(0, 1, 0) : new Color(1, 0, 0);
    }

    public bool Build()
    {
        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(BuildingDataManager.Instance.GetBuildingData(Type), GlobalPosition);
        bool canPlace = GameManager.Instance.BuildingManager.CanPlaced(BuildingDataManager.Instance.GetBuildingData(Type), snapPos);
        if (canPlace)
        {
            GameManager.Instance.BuildingManager.Place(BuildingDataManager.Instance.GetBuildingData(Type), snapPos);

            switch (Type)
            {
                case BuildingType.MainBase:
                    BuildingBase bd = _buildingBasePs.Instantiate<BuildingBase>();
                    bd.Init(Type, snapPos);//没有特殊功能的, 用buildingBase脚本, 走普通初始化函数
                    GetTree().CurrentScene.AddChild(bd);
                    break;
            }
            QueueFree();
            return true;
        }
        return false;
    }


}
