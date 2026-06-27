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
        public TargetType TargetType;
        [Export] private Node2D _animRoot;
        [Export] private Sprite2D _sprite;
        [Export] private HpBar _hpBar;
        public float _maxHp = 100;
        protected float _curHp;
        private List<(ItemType, int, int)> _dropItemList;//掉落物类型, 最小掉落数量, 最大掉落数量
        private ShaderMaterial _shaderMaterial;

        public virtual void Init(BuildingType type, Vector2 snapPos)
        {
            Type = type;
            GlobalPosition = snapPos;
            BuildingData buildingData = BuildingDataManager.Instance.GetBuildingData(Type);
            TargetType = buildingData.TargetType;
            SetMaxHp(buildingData.MaxHp);
            SetCurHp(_maxHp);
            _dropItemList = buildingData.DropItemList;
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
            if (_curHp > _maxHp)
                _curHp = _maxHp;
            SetCurHp(_curHp);
        }

        public Vector2 GetWorldPosition()
        {
            return GlobalPosition;
        }
        public void TakeDamage(Node2D atker, float damage, ItemType? itemType)
        {
            Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            animTween.TweenProperty(_animRoot, "scale", new Vector2(0.8f, 0.8f), 0.1f);
            animTween.Parallel().TweenProperty(_sprite.Material, "shader_parameter/flash_modifier", 1.0f, 0.1f);
            animTween.TweenProperty(_animRoot, "scale", new Vector2(1.2f, 1.2f), 0.1f);
            animTween.Parallel().TweenProperty(_sprite.Material, "shader_parameter/flash_modifier", 0.0f, 0.1f);
            animTween.TweenProperty(_animRoot, "scale", new Vector2(1f, 1f), 0.1f);



            _damageCooldownTimer = 0f;
            _healTimer = 0f;

            float finalDamage = HandleDamage(damage, itemType);//子类重写, 默认不处理

            FloatTextLb floatTextLb = GameManager.Instance.FloatTextLbPs.Instantiate<FloatTextLb>();
            GetTree().CurrentScene.AddChild(floatTextLb);
            floatTextLb.Init($"-{finalDamage}", GlobalPosition, new Color(162 / 256f, 38 / 256f, 51 / 256f));

            SetCurHp(_curHp - finalDamage);
            if (_curHp <= 0)
            {
                Die();//子类重写, 默认爆装备
            }
        }


        protected virtual float HandleDamage(float damage, ItemType? itemType)
        {
            return damage;
        }
        protected virtual void Die()
        {
            SetCurHp(0);
            //掉落物品
            foreach ((ItemType, int, int) tuple in _dropItemList)
            {
                DropItem dropItem = GameManager.Instance.DropItemPs.Instantiate<DropItem>();
                GetTree().CurrentScene.AddChild(dropItem);
                dropItem.Init(new ItemInstance() { Type = tuple.Item1, Count = Random.Shared.Next(tuple.Item2, tuple.Item3 + 1) }, Position);
                dropItem.ApplyForce();
            }
            GameManager.Instance.BuildingManager.Remove(Type, GlobalPosition);
            GameManager.Instance.ChunkManager.RemoveItem(this, GlobalPosition);
            QueueFree();
        }


        protected void SetMaxHp(float maxHp)
        {
            _maxHp = maxHp;
            _hpBar.Refresh(_curHp, _maxHp);
        }
        protected void SetCurHp(float curHp)
        {
            _curHp = curHp;
            _hpBar.Refresh(_curHp, _maxHp);
        }

        public bool CanInteract()//todo : 根据配置项定
        {
            return true;
        }

        public bool CanAtk()
        {
            return true;
        }

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

        public virtual void Interact()
        {
            GD.Print("Building Interact");
            return;
        }

        public bool IsVaild()
        {
            return IsInstanceValid(this);
        }

        public TargetType GetTargetType()
        {
            return TargetType;
        }
    }
}