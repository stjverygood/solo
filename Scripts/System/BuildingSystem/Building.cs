using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.System.ItemSystem;
using System;
using System.Collections.Generic;

namespace Solo.Scripts.System.BuildingSystem
{
    public partial class Building : StaticBody2D, ITargetable
    {
        public BuildingType Type;
        [Export] private CollisionShape2D _collisionShape;
        [Export] private Node2D _animRoot;
        [Export] private Sprite2D _sprite;
        [Export] public ProgressBar _hpPb;
        [Export] public Label _hpLb;
        [Export] public PackedScene DropItemPs;
        [Export] private NavigationObstacle2D _naviObstacle;
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

            _sprite.Texture = GD.Load<Texture2D>(buildingData.TexturePath);
            _sprite.Position = new Vector2(0, -(buildingData.TextureHeight / 2f - buildingData.Height) * 16);
            if (_collisionShape.Shape is RectangleShape2D rectShape)
            {
                RectangleShape2D uniqueRectShape = (RectangleShape2D)rectShape.Duplicate();
                uniqueRectShape.Size = new Vector2(buildingData.Width * 16f, buildingData.Height * 16f); // 赋予新的尺寸
                _collisionShape.Shape = uniqueRectShape;
                _collisionShape.Position = new Vector2(0, (buildingData.Height / 2f) * 16);
            }
            if (_naviObstacle != null)
            {
                float halfW = buildingData.Width * 16 / 2f;
                float halfH = buildingData.Height * 16 / 2f;
                Vector2[] obstacleVertices = new Vector2[] { new Vector2(-halfW, -halfH), new Vector2(halfW, -halfH), new Vector2(halfW, halfH), new Vector2(-halfW, halfH) };// 严格按照顺时针（Clockwise）定义矩形的 4 个顶点
                _naviObstacle.Vertices = obstacleVertices;
                _naviObstacle.AvoidanceEnabled = true;// 确保避障属性正确开启
                _naviObstacle.AvoidanceLayers = 1; // 保持和之前 Units 的 AvoidanceMask 一致
                _naviObstacle.Position = new Vector2(0, (buildingData.Height / 2f) * 16);
            }


            if (_sprite.Material is ShaderMaterial shaderMat)
            {
                _shaderMaterial = (ShaderMaterial)shaderMat.Duplicate();// 关键：复制一份材质，确保每个实例的材质相互独立
                _sprite.Material = _shaderMaterial;// 记得把复制后的独立材质重新赋给当前的 Sprite2D
            }
            RefreshHpBar();
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
        private float _healDuration = 0.1f;
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
            RefreshHpBar();
        }



        //public virtual void ShowOutline(bool isShow)
        //{
        //    if (isShow)
        //    {
        //        _shaderMaterial.SetShaderParameter("outline_color", new Godot.Color(1, 1, 1));
        //        _shaderMaterial.SetShaderParameter("outline_width", 1);
        //    }
        //    else
        //    {
        //        _shaderMaterial.SetShaderParameter("outline_width", 0.0f);
        //    }
        //}

        private void RefreshHpBar()
        {
            if (_curHp == MaxHp)
            {
                _hpPb.Visible = false;
                return;
            }
            _hpPb.Position = new Vector2(_hpPb.Position.X, _sprite.Position.Y - (BuildingDataManager.Instance.GetBuildingData(Type).TextureHeight * 16 / 2 + 8));
            _hpPb.Visible = true;
            _hpPb.MaxValue = MaxHp;
            _hpPb.Value = _curHp;
            _hpLb.Text = $"{_curHp:f0}/{MaxHp:f0}";
        }


        public Vector2 GetWorldPosition()
        {
            return GlobalPosition;
        }
        public void TakeDamage(float damage, ItemType? itemType)
        {
            damage = HandleDamage(itemType, damage);
            Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            animTween.TweenProperty(_animRoot, "scale", new Vector2(0.8f, 0.8f), 0.1f);
            animTween.Parallel().TweenProperty(_sprite.Material, "shader_parameter/flash_modifier", 1.0f, 0.1f);
            animTween.TweenProperty(_animRoot, "scale", new Vector2(1.2f, 1.2f), 0.1f);
            animTween.Parallel().TweenProperty(_sprite.Material, "shader_parameter/flash_modifier", 0.0f, 0.1f);
            animTween.TweenProperty(_animRoot, "scale", new Vector2(1f, 1f), 0.1f);
            FloatTextLb floatTextLb = GameManager.Instance.FloatTextLbPs.Instantiate<FloatTextLb>();
            GetTree().CurrentScene.AddChild(floatTextLb);
            floatTextLb.Init($"-{damage}", GlobalPosition);
            _curHp -= damage;
            _damageCooldownTimer = 0f;
            _healTimer = 0f;
            RefreshHpBar();
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
        private float HandleDamage(ItemType? dmgItemType, float damage)
        {
            float resDmg = damage;
            switch (Type)
            {
                case BuildingType.Tree:
                    switch (dmgItemType)
                    {
                        case ItemType.WoodAxe:
                            resDmg *= 2;
                            break;
                        case ItemType.IronAxe:
                            resDmg *= 3;
                            break;
                        case ItemType.GoldAxe:
                            resDmg *= 4;
                            break;
                        case ItemType.JadeAxe:
                            resDmg *= 5;
                            break;
                        default:
                            resDmg *= 1;
                            break;
                    }
                    break;
                case BuildingType.Stone:
                    switch (dmgItemType)
                    {
                        case ItemType.WoodPickaxe:
                            resDmg *= 1;
                            break;
                        case ItemType.IronPickaxe:
                            resDmg *= 2;
                            break;
                        case ItemType.GoldPickaxe:
                            resDmg *= 3;
                            break;
                        case ItemType.JadePickaxe:
                            resDmg *= 4;
                            break;
                        default:
                            resDmg *= 0.1f;
                            break;
                    }
                    break;
            }
            return resDmg;
        }

        public bool CanInteract()//todo : 根据配置项定
        {
            return false;
        }

        public bool CanAtk()
        {
            return true;
        }

        //public void ShowInteractTip(bool isShow)
        //{
        //    return;
        //}

        //public void ShowAtkTip(bool isShow)
        //{
        //    if (isShow)
        //    {
        //        _shaderMaterial.SetShaderParameter("outline_color", new Godot.Color(1, 1, 1));
        //        _shaderMaterial.SetShaderParameter("outline_width", 1);
        //    }
        //    else
        //    {
        //        _shaderMaterial.SetShaderParameter("outline_width", 0.0f);
        //    }
        //}

        public void ShowOutline(bool isShow)
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

        public void Interact()
        {
            return;
        }

        public bool IsVaild()
        {
            return IsInstanceValid(this);
        }
    }
}