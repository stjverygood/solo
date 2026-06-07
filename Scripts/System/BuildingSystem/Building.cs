using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.BuildingSystem;
using Solo.Scripts.System.ItemSystem;
using System;
using System.Collections.Generic;

public partial class Building : StaticBody2D
{
    public BuildingType Type;
    [Export] private CollisionShape2D _collisionShape;
    [Export] private Node2D _animRoot;
    [Export] private Sprite2D _sprite;
    [Export] public ProgressBar _hpPb;
    [Export] public PackedScene DropItemPs;
    public float MaxHp = 100;
    private List<(ItemType, int, int)> _dropItemList;//掉落物类型, 最小掉落数量, 最大掉落数量
    private float _curHp;
    private ShaderMaterial _shaderMaterial;

    public void Init(BuildingType type, Vector2 snapPos)
    {
        Type = type;
        GlobalPosition = snapPos;
        BuildingData buildingData = BuildingDataManager.Instance.GetBuildingData(Type);
        MaxHp = buildingData.MaxHp;
        _curHp = MaxHp;
        _dropItemList = buildingData.DropItemList;
        if (_collisionShape.Shape is RectangleShape2D rectShape)
        {
            RectangleShape2D uniqueShape = (RectangleShape2D)rectShape.Duplicate();// 关键：复制一份 Shape 资源，确保当前建筑的碰撞体独立
            uniqueShape.Size = new Vector2(buildingData.Width * 16, buildingData.Height * 16);// 赋予新的尺寸
            _collisionShape.Shape = uniqueShape;// 把独立后的 Shape 重新赋给当前建筑的 CollisionShape2D
        }
        _sprite.Texture = GD.Load<Texture2D>(buildingData.TexturePath);
        if (_sprite.Material is ShaderMaterial shaderMat)
        {
            _shaderMaterial = (ShaderMaterial)shaderMat.Duplicate();// 关键：复制一份材质，确保每个实例的材质相互独立
            _sprite.Material = _shaderMaterial;// 记得把复制后的独立材质重新赋给当前的 Sprite2D
        }
        ShowOutline(false);
        GameManager.Instance.BuildingManager.Place(Type, snapPos);
        GameManager.Instance.ChunkManager.AddItem(this, GlobalPosition);
    }

    public override void _Process(double delta)
    {
        AutoHeal((float)delta);
    }

    private float _damageCooldownTimer = 0; // 受伤后的冷却计时器
    private const float _healDelayAfterDamage = 3.0f; // 受伤后等待 3 秒才开始回血
    private float _healTimer = 0;
    private float _healDuration = 1;
    private void AutoHeal(float delta)
    {
        if (_damageCooldownTimer < _healDelayAfterDamage)
        {
            _damageCooldownTimer += delta;
            return;// 如果距离上次受伤还没过去 3 秒，就累加时间并直接返回（不回血）
        }
        _healTimer += delta;
        if (_healTimer < _healDuration)
            return;
        _healTimer = 0;
        _curHp++;
        if (_curHp > MaxHp)
            _curHp = MaxHp;
        //_hpBarControl.Refresh(MaxHp, _curHp);
    }

    public virtual void TakeDamage(ItemType? dmgItemType, float damage)
    {
        Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        animTween.TweenProperty(_animRoot, "skew", 0.2f, 0.1f);
        animTween.Parallel().TweenProperty(_sprite.Material, "shader_parameter/flash_modifier", 1.0f, 0.1f);
        animTween.TweenProperty(_animRoot, "skew", -0.2f, 0.1f);
        animTween.Parallel().TweenProperty(_sprite.Material, "shader_parameter/flash_modifier", 0.0f, 0.1f);
        animTween.TweenProperty(_animRoot, "skew", 0f, 0.1f);
        FloatTextLb floatTextLb = GameManager.Instance.FloatTextLbPs.Instantiate<FloatTextLb>();
        GetTree().CurrentScene.AddChild(floatTextLb);
        floatTextLb.Init($"-{damage}", GlobalPosition);
        GameManager.Instance.Player.TriggerScreenShake(5f);
        _curHp -= damage;
        _damageCooldownTimer = 0f;
        _healTimer = 0f;
        //_hpBarControl.Refresh(MaxHp, _curHp);
        if (_curHp <= 0)
        {
            GameManager.Instance.ChunkManager.RemoveItem(this, GlobalPosition);
            _curHp = 0;
            //掉落物品
            foreach ((ItemType, int, int) tuple in _dropItemList)
            {
                DropItem dropItem = DropItemPs.Instantiate<DropItem>();
                GetTree().CurrentScene.AddChild(dropItem);
                dropItem.Init(tuple.Item1, Random.Shared.Next(tuple.Item2, tuple.Item3 + 1), Position);
                dropItem.ApplyForce();
            }
            QueueFree();
            GameManager.Instance.BuildingManager.Remove(Type, GlobalPosition);
            GameManager.Instance.ChunkManager.RemoveItem(this, GlobalPosition);
        }
    }

    public virtual void ShowOutline(bool isShow)
    {
        if (isShow)
        {
            _shaderMaterial.SetShaderParameter("outline_color", new Godot.Color(1, 1, 1));
            _shaderMaterial.SetShaderParameter("outline_width", 1);
        }
        else
        {
            _shaderMaterial.SetShaderParameter("outline_width", 0.0f);
        }
    }
}
