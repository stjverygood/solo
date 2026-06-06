using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.BuildingSystem;

public partial class BuildingPreview : Node2D
{
    public BuildingType Type;
    [Export] private Sprite2D _sprite;
    [Export] private PackedScene _buildingPs;
    private Vector2 _curDir = Vector2.Down;

    public void Init(ItemType itemType, Vector2 playerPos)
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
        Update(playerPos);
    }

    //public override void _PhysicsProcess(double delta)
    //{

    //}


    public void Update(Vector2 playerPos)
    {
        BuildingData buildingData = BuildingDataManager.Instance.GetBuildingData(Type);
        Vector2 worldPos = playerPos + new Vector2(_curDir.X * buildingData.Width / 2 * 20, _curDir.Y * buildingData.Height / 2 * 20);
        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(Type, worldPos);
        bool canPlace = GameManager.Instance.BuildingManager.CanPlaced(Type, snapPos);
        GlobalPosition = snapPos;
        _sprite.Modulate = canPlace ? new Color(0, 1, 0, 0.2f) : new Color(1, 0, 0, 0.2f);
    }

    public void ChangeDir()
    {
        if (_curDir == Vector2.Down)
        {
            _curDir = Vector2.Left;
            return;
        }
        if (_curDir == Vector2.Left)
        {
            _curDir = Vector2.Up;
            return;
        }
        if (_curDir == Vector2.Up)
        {
            _curDir = Vector2.Right;
            return;
        }
        if (_curDir == Vector2.Right)
        {
            _curDir = Vector2.Down;
            return;
        }
    }

    public bool Build()
    {
        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(Type, GlobalPosition);
        bool canPlace = GameManager.Instance.BuildingManager.CanPlaced(Type, snapPos);
        if (canPlace)
        {
            GameManager.Instance.BuildingManager.Place(Type, snapPos);

            switch (Type)
            {
                case BuildingType.MainBase:
                    Building bd = _buildingPs.Instantiate<Building>();
                    bd.Init(Type, snapPos);//没有特殊功能的, 用buildingBase脚本, 走普通初始化函数
                    GetTree().CurrentScene.AddChild(bd);
                    break;
            }
            return true;
        }
        return false;
    }


}
