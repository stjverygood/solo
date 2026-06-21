using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.System.BuildingSystem;
using Solo.Scripts.System.BuildingSystem.Buildings;

public partial class BuildingPreview : Node2D
{
    public BuildingType Type;
    [Export] private Sprite2D _sprite;
    private Vector2 _curDir = Vector2.Down;
    private bool _canPlace = false;

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
            case ItemType.DefendCraft:
                Type = BuildingType.DefendCraft;
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
        RefreshPosition(mousePos);
    }

    public void RefreshPosition(Vector2 mousePos)
    {
        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(Type, mousePos);
        _canPlace = GameManager.Instance.BuildingManager.CanPlaced(Type, snapPos);
        if (_canPlace && Type != BuildingType.MainBase)//非主基地要多进行一次灵气范围校验
        {
            bool isInQiRange = false;
            foreach (IQiRangeable qiRangeable in GameManager.Instance.IQiRangeableList)
            {
                if (snapPos.DistanceTo(qiRangeable.GetWorldPos()) <= qiRangeable.GetQiRange())
                {
                    isInQiRange = true;
                    break;
                }
            }
            _canPlace = isInQiRange;
        }

        GlobalPosition = snapPos;
        _sprite.Modulate = _canPlace ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 0, 0.4f);
    }

    public bool Build(Vector2 mousePos)
    {
        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(Type, mousePos);
        //bool canPlace = GameManager.Instance.BuildingManager.CanPlaced(Type, snapPos);
        if (!_canPlace)
            return false;
        GameManager.Instance.BuildingManager.Place(Type, snapPos);

        BuildingData buildingData = BuildingDataManager.Instance.GetBuildingData(Type);

        switch (Type)
        {
            case BuildingType.MainBase:
                PackedScene mainBasePs = GD.Load<PackedScene>(buildingData.TscnPath);
                MainBase mainBase = mainBasePs.Instantiate<MainBase>();
                mainBase.Init(Type, GlobalPosition);
                mainBase.ShowQiRange(true);
                GetTree().CurrentScene.AddChild(mainBase);
                break;
            case BuildingType.BuildingCraft:
                PackedScene buildingCraftPs = GD.Load<PackedScene>(buildingData.TscnPath);
                BuildingCraft buildingCraft = buildingCraftPs.Instantiate<BuildingCraft>();
                buildingCraft.Init(Type, GlobalPosition);
                GetTree().CurrentScene.AddChild(buildingCraft);
                break;
            case BuildingType.Flag:
                PackedScene flagPs = GD.Load<PackedScene>(buildingData.TscnPath);
                Flag flag = flagPs.Instantiate<Flag>();
                flag.Init(Type, GlobalPosition);
                flag.ShowQiRange(true);
                GetTree().CurrentScene.AddChild(flag);
                break;
            case BuildingType.ToolCraft:
                PackedScene toolCraftPs = GD.Load<PackedScene>(buildingData.TscnPath);
                ToolCraft toolCraft = toolCraftPs.Instantiate<ToolCraft>();
                toolCraft.Init(Type, GlobalPosition);
                GetTree().CurrentScene.AddChild(toolCraft);
                break;
            case BuildingType.DefendCraft:
                PackedScene defendCraftPs = GD.Load<PackedScene>(buildingData.TscnPath);
                DefendCraft defendCraft = defendCraftPs.Instantiate<DefendCraft>();
                defendCraft.Init(Type, GlobalPosition);
                GetTree().CurrentScene.AddChild(defendCraft);
                break;
        }
        return true;
    }

    //private bool Check
}
