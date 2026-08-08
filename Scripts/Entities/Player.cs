using Godot;
using Solo.Scripts.Components.ExpComponents;
using Solo.Scripts.Components.HpComponents;
using Solo.Scripts.Components.InventoryComponents;
using Solo.Scripts.Components.OutlineComponents;
using Solo.Scripts.Components.PickableComponents;
using Solo.Scripts.Components.QiComponents;
using Solo.Scripts.Components.RealmComponents;
using Solo.Scripts.Components.StartPositionComponents;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.Projectiles;
using Solo.Scripts.System.ItemSystem;
using System;


namespace Solo.Scripts.Entities
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Dash,
        Atk,//攻击
        PreInteract,
        Interact,//交互
        Build,//建造状态
        Comsume,//消耗物品
        Aim,//瞄准状态
        Fishing,
        Death,
        BagUI,
    }

    public partial class Player : CharacterBody2D, IEntity
    {
        private bool _initialized = false;
        public PlayerState CurState;
        private Vector2 _curMoveDir = Vector2.Right;//移动朝向, 记录最后一次移动输入方向
        //private Vector2 _curFaceDir = Vector2.Right;//脸部方向, 记录鼠标方向
        [Export] private Camera2D _camera = null!;

        [Export] private AnimatedSprite2D _animSprite = null!;

        [Export] private Node2D _handRootNode = null!;//手部根节点
        [Export] private Sprite2D _handSprite = null!;//手持物sprite


        //[Export] private PackedScene _fishingFloatPs = null!;

        //人物属性
        //public Vector2 StartPoint = new Vector2(0, 0);//出生点
        private float _moveSpeed = 100;

        public int CurFastBarIndex;
        public event Action? OnCurFastBarIndexChanged;

        [Export] private Label _debugLb = null!;
        //[Export] private DeathView _deathView = null!;
        [Export] public Texture2D _aimIconTexture = null!;
        [Export] public Texture2D _interactIconTexture = null!;

        //[Export] AnimatedSprite2D _bgAnimSprite = null!;

        public EntityCore Core { get; private set; } = null!;

        public void Init(EntityType type, EntitySaveData? entitySaveData)
        {
            GD.Print("Player Init~~~");


            GameManager.Instance.Player = this;
            Core = new EntityCore(type);
            Core.InitComponent(this, entitySaveData);
            _initialized = true;

            Core.GetComponent<InventoryComponent>().FastBarInventory.SlotChanged += (i) =>
            {
                RefreshHandNode();
            };
            RefreshHandNode();

            if (Core.GetComponent<HpComponent>().CurHp == 0)//若血量是0, 进入重生逻辑
                Revive();
            ChangeState(PlayerState.Idle);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (!_initialized) return;

            //if (CurState != PlayerState.BagUI && Input.IsActionJustPressed("Back"))
            //    GameManager.Instance.UIManager.PauseGame();

            UpdateState((float)delta);
            _debugLb.Text = CurState.ToString();
            //GD.Print($"GetTileType(GetGlobalMousePosition()) : {GameManager.Instance.ChunkManager.GetTileType(GetGlobalMousePosition())}");
        }

        public override void _UnhandledInput(InputEvent @event)
        {

            if (@event is InputEventKey keyEvent && keyEvent.Pressed)
            {
                switch (keyEvent.Keycode)
                {
                    case Key.Escape:
                        if (CurState == PlayerState.BagUI)
                        {
                            ChangeState(PlayerState.Idle);
                            return;
                        }
                        else
                        {
                            GameManager.Instance.UIManager.PauseView.Visible = true;
                            GetTree().Paused = true;
                        }
                        break;
                    case Key.Key1:// 测试用
                        //GetHp(10);
                        GD.Print("快捷检测：按下了 1");
                        Core.GetComponent<ExpComponent>().Gain(10000);
                        break;
                    case Key.Key2:
                        Core.GetComponent<HpComponent>().Gain(100);
                        GD.Print("快捷检测：按下了 2");
                        break;
                    case Key.Key3:
                        Core.GetComponent<HpComponent>().Consume(100);
                        GD.Print("快捷检测：按下了 3");
                        break;
                    case Key.Key4:
                        //TakeMp(10);
                        GD.Print("快捷检测：按下了 4");
                        break;
                }
            }
        }

        private void ChangeState(PlayerState newState)
        {
            ExitState(CurState);
            CurState = newState;
            EnterState(newState);
        }

        private void EnterState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    EnterIdle();
                    break;
                case PlayerState.Walk:
                    EnterWalk();
                    break;
                case PlayerState.Run:
                    EnterRun();
                    break;
                case PlayerState.Dash:
                    EnterDash();
                    break;
                case PlayerState.Atk:
                    EnterAtk();
                    break;
                case PlayerState.PreInteract:
                    EnterPreInteract();
                    break;
                case PlayerState.Interact:
                    EnterInteract();
                    break;
                case PlayerState.Build:
                    EnterBuild();
                    break;
                case PlayerState.Comsume:
                    EnterComsume();
                    break;
                case PlayerState.Aim:
                    EnterAim();
                    break;
                case PlayerState.Fishing:
                    EnterFishing();
                    break;
                case PlayerState.Death:
                    EnterDeath();
                    break;
                case PlayerState.BagUI:
                    EnterBagUI();
                    break;
            }
        }
        private void UpdateState(float delta)
        {
            switch (CurState)
            {
                case PlayerState.Idle:
                    UpdateIdle(delta);
                    break;
                case PlayerState.Walk:
                    UpdateWalk(delta);
                    break;
                case PlayerState.Run:
                    UpdateRun(delta);
                    break;
                case PlayerState.Dash:
                    UpdateDash(delta);
                    break;
                case PlayerState.Atk:
                    UpdateAtk(delta);
                    break;
                case PlayerState.PreInteract:
                    UpdatePreInteract(delta);
                    break;
                case PlayerState.Interact:
                    UpdateInteract(delta);
                    break;
                case PlayerState.Build:
                    UpdateBuild(delta);
                    break;
                case PlayerState.Comsume:
                    UpdateComsume(delta);
                    break;
                case PlayerState.Aim:
                    UpdateAim(delta);
                    break;
                case PlayerState.Fishing:
                    UpdateFishing(delta);
                    break;
                case PlayerState.Death:
                    UpdateDeath(delta);
                    break;
                case PlayerState.BagUI:
                    UpdateBagUI(delta);
                    break;
            }
        }
        private void ExitState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    ExitIdle();
                    break;
                case PlayerState.Walk:
                    ExitWalk();
                    break;
                case PlayerState.Run:
                    ExitRun();
                    break;
                case PlayerState.Dash:
                    ExitDash();
                    break;
                case PlayerState.Atk:
                    ExitAtk();
                    break;
                case PlayerState.PreInteract:
                    ExitPreInteract();
                    break;
                case PlayerState.Interact:
                    ExitInteract();
                    break;
                case PlayerState.Build:
                    ExitBuild();
                    break;
                case PlayerState.Comsume:
                    ExitComsume();
                    break;
                case PlayerState.Aim:
                    ExitAim();
                    break;
                case PlayerState.Fishing:
                    ExitFishing();
                    break;
                case PlayerState.Death:
                    ExitDeath();
                    break;
                case PlayerState.BagUI:
                    ExitBagUI();
                    break;
            }
        }

        #region Idle
        private void EnterIdle()
        {
            //ResetAnim();

            //_animTween = CreateTween().SetLoops();
            //_animTween.TweenProperty(_animRootNode, "scale", new Vector2(1.02f, 0.98f), 0.2)
            //    .SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            //_animTween.Chain().TweenProperty(_animRootNode, "scale", new Vector2(0.98f, 1.02f), 0.2)
            //    .SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
            _animSprite.Play("Idle");
        }
        private void UpdateIdle(float delta)
        {
            if (Core.GetComponent<HpComponent>().CurHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }

            if (Input.IsActionJustPressed("Bag"))
            {
                ChangeState(PlayerState.BagUI);
                return;
            }

            if (Input.IsActionJustPressed("Dash"))
            {
                ChangeState(PlayerState.Dash);
                return;
            }

            //ConsumeDuration(delta, _mpConsume * 1);
            if (Input.IsActionJustPressed("Pre"))
                ChangeCurFastBarIndex(false);
            if (Input.IsActionJustPressed("Next"))
                ChangeCurFastBarIndex(true);

            if (Input.IsActionJustPressed("Atk"))
            {
                if (Core.GetComponent<InventoryComponent>().IsCurItemTypeExist(CurFastBarIndex) == false)
                    return;
                ItemData itemData = ItemDataManager.Instance.GetData(Core.GetComponent<InventoryComponent>().GetCurItemType(CurFastBarIndex));
                if (itemData.CanAim)
                {
                    ChangeState(PlayerState.Aim);
                    return;
                }
                if (itemData.CanBuild)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
            }

            if (Input.IsActionJustPressed("Interact"))
            {
                ChangeState(PlayerState.PreInteract);
                return;
            }

            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input != Vector2.Zero)
            {
                ChangeState(PlayerState.Walk);
                return;
            }

            RefreshFaceDir();
        }
        private void ExitIdle()
        {

        }
        #endregion

        #region Walk
        private void EnterWalk()
        {
            //ResetAnim();
            //_animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            //_animTween.TweenProperty(_animRootNode, "skew", 0.1f, 0.3f);// 走动效果：左右晃动或轻微拉伸
            //_animTween.TweenProperty(_animRootNode, "skew", -0.1f, 0.3f);

            _animSprite.Play("Move");
        }
        private void UpdateWalk(float delta)
        {
            if (Core.GetComponent<HpComponent>().CurHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }

            if (Input.IsActionJustPressed("Bag"))
            {
                ChangeState(PlayerState.BagUI);
                return;
            }

            if (Input.IsActionJustPressed("Dash"))
            {
                ChangeState(PlayerState.Dash);
                return;
            }

            if (Input.IsActionJustPressed("Atk"))
            {
                if (Core.GetComponent<InventoryComponent>().IsCurItemTypeExist(CurFastBarIndex) == false)
                    return;
                ItemData itemData = ItemDataManager.Instance.GetData(Core.GetComponent<InventoryComponent>().GetCurItemType(CurFastBarIndex));
                if (itemData.CanAim)
                {
                    ChangeState(PlayerState.Aim);
                    return;
                }
                if (itemData.CanBuild)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
            }

            if (Input.IsActionJustPressed("Interact"))
            {
                ChangeState(PlayerState.PreInteract);
                return;
            }

            //ConsumeDuration(delta, _mpConsume * 1.2f);
            if (Input.IsActionJustPressed("Pre"))
                ChangeCurFastBarIndex(false);
            if (Input.IsActionJustPressed("Next"))
                ChangeCurFastBarIndex(true);

            Vector2 moveInput = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (moveInput == Vector2.Zero)
            {
                ChangeState(PlayerState.Idle);
                return;
            }

            RefreshFaceDir();

            _curMoveDir = moveInput;
            //if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
            //{
            //    Velocity = _curMoveDir * _moveSpeed / 4;
            //    _walkGrassParticles.Emitting = false;
            //    _walkWaterParticles.Emitting = true;
            //}
            //else
            //{
            //    Velocity = _curMoveDir * _moveSpeed;
            //    _walkGrassParticles.Emitting = true;
            //    _walkWaterParticles.Emitting = false;
            //}
            Velocity = _curMoveDir * _moveSpeed;
            MoveAndSlide();
        }
        private void ExitWalk()
        {
        }
        #endregion

        #region Run
        private void EnterRun()
        {
            //ResetAnim();
            //_animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            //_animTween.TweenProperty(_animRootNode, "skew", 0.1f, 0.1f);// 走动效果：左右晃动或轻微拉伸
            //_animTween.TweenProperty(_animRootNode, "skew", -0.1f, 0.1f);

            _animSprite.Play("Move");
        }
        private void UpdateRun(float delta)
        {
            if (Core.GetComponent<HpComponent>().CurHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }

            if (Input.IsActionJustPressed("Atk"))
            {
                if (Core.GetComponent<InventoryComponent>().IsCurItemTypeExist(CurFastBarIndex) == false)
                    return;
                ItemData itemData = ItemDataManager.Instance.GetData(Core.GetComponent<InventoryComponent>().GetCurItemType(CurFastBarIndex));
                if (itemData.CanAim)
                {
                    ChangeState(PlayerState.Aim);
                    return;
                }
                if (itemData.CanBuild)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
            }

            if (Input.IsActionJustPressed("Interact"))
            {
                ChangeState(PlayerState.PreInteract);
                return;
            }

            //ConsumeDuration(delta, _mpConsume * 2);
            if (Input.IsActionJustPressed("Pre"))
                ChangeCurFastBarIndex(false);
            if (Input.IsActionJustPressed("Next"))
                ChangeCurFastBarIndex(true);

            RefreshFaceDir();

            Vector2 moveInput = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (moveInput == Vector2.Zero || Input.IsActionPressed("Dash") == false)
            {
                ChangeState(PlayerState.Idle);
                return;
            }
            _curMoveDir = moveInput;
            //if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
            //    Velocity = _curMoveDir * _moveSpeed / 2;
            //else
            Velocity = _curMoveDir * _moveSpeed * 2;
            MoveAndSlide();
        }
        private void ExitRun()
        {
        }
        #endregion

        #region Dash
        private float _dashTimer;
        private float _dashDuration = 0.2f;
        private float _dashSpeed = 200;
        private void EnterDash()
        {
            //ResetAnim();
            //_animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            //_animTween.TweenProperty(_animRootNode, "skew", 0.1f, 0.3f);// 走动效果：左右晃动或轻微拉伸
            //_animTween.TweenProperty(_animRootNode, "skew", -0.1f, 0.3f);
            //_dashTimer = 0;
            //TakeMp(1);

            _animSprite.Play("Action");
        }
        private void UpdateDash(float delta)
        {
            if (Core.GetComponent<HpComponent>().CurHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }

            _dashTimer += delta;
            if (_dashTimer >= _dashDuration)
            {
                if (Input.IsActionPressed("Dash"))
                {
                    ChangeState(PlayerState.Run);
                    return;
                }
                else
                {
                    ChangeState(PlayerState.Idle);
                    return;
                }
            }

            RefreshFaceDir();

            ConsumeDuration(delta, 0.3f);
            if (Input.IsActionJustPressed("Pre"))
                ChangeCurFastBarIndex(false);
            if (Input.IsActionJustPressed("Next"))
                ChangeCurFastBarIndex(true);

            Velocity = _curMoveDir * _dashSpeed;
            MoveAndSlide();
        }
        private void ExitDash()
        {

        }
        #endregion

        #region Atk
        private void EnterAtk()
        {
            //ResetAnim();
            //_animSprite.Play("Action");

            float startRotation = _handRootNode.Rotation;
            float attackRotation = startRotation + 1.3f; // 增加1.3弧度
            Tween _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            _animTween.TweenProperty(_handRootNode, "rotation", attackRotation, 0.05f);

            //Vector2 atkDir = (GetGlobalMousePosition() - GlobalPosition).Normalized();

            var projectileContext = new ProjectileContext() { Projecter = this, StartPos = GlobalPosition, TargetPos = GetGlobalMousePosition() };

            switch (Core.GetComponent<InventoryComponent>().GetCurItemType(CurFastBarIndex))
            {
                case ItemType.WoodSword:
                    GameManager.Instance.SpawnProjectile(ProjectileType.SwordWave, projectileContext);
                    break;
                case ItemType.WoodAxe:
                    //_animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                    //_animTween.TweenProperty(_handRootNode, "rotation", attackRotation, 0.05f);

                    //HammerWave hammerWave = _hammerWavePs.Instantiate<HammerWave>();
                    //GetTree().CurrentScene.AddChild(hammerWave);
                    //hammerWave.Init(GlobalPosition, atkDir);


                    //_animTween.Parallel().TweenProperty(_animRootNode, "scale", new Vector2(1f, 1f), 0.1f);
                    //_animTween.TweenProperty(_handRootNode, "rotation", startRotation, 0.1f);
                    //_animTween.Finished += () =>
                    //{
                    //    ChangeState(PlayerState.Idle);
                    //    return;
                    //};
                    break;
            }


            _animTween.TweenProperty(_handRootNode, "rotation", startRotation, 0.1f);
            _animTween.Finished += () =>
            {
                ChangeState(PlayerState.Idle);
                return;
            };
        }
        private void UpdateAtk(float delta)
        {
            if (Core.GetComponent<HpComponent>().CurHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }

            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input != Vector2.Zero)
            {
                Velocity = input * _moveSpeed;
                MoveAndSlide();
            }
        }
        private void ExitAtk()
        {

        }
        #endregion

        #region PreInteract
        private IEntity? _curTargetEntity = null;
        private void EnterPreInteract()
        {
            //ResetAnim();
            _animSprite.Play("Idle");
            Input.SetCustomMouseCursor(_interactIconTexture, Input.CursorShape.Arrow, _aimIconTexture.GetSize() / 2);
        }
        private void UpdatePreInteract(float delta)
        {
            if (Input.IsActionJustReleased("Atk"))
            {
                ChangeState(PlayerState.Idle);
                return;
            }

            if (Input.IsActionJustReleased("Interact"))
            {
                if (_curTargetEntity == null || !IsInstanceValid((Node2D)_curTargetEntity))
                {
                    ChangeState(PlayerState.Idle);
                    return;
                }
                if (_curTargetEntity.Core.TryGetComponent<PickableComponent>(out var pickableComp) == true)//掉落物, 直接捡起来, 不用有状态
                {
                    if (Core.GetComponent<InventoryComponent>().AddItem(pickableComp.ItemInstance) == 0)
                    {
                        ((Node2D)_curTargetEntity).QueueFree();//捡完了
                    }
                    ChangeState(PlayerState.Idle);
                    return;
                }
                //if ()
                //    ChangeState(PlayerState.Interact);//建筑/npc之类的, 给个
                return;
            }

            //每帧刷新_curTargetEntity, 把信息显示出来
            RefreshCurTargetEntity();
            RefreshFaceDir();
            //Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            //if (input != Vector2.Zero)
            //    _curMoveDir = input;
            //if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
            //    Velocity = input * _moveSpeed / 4;
            //else
            //    Velocity = input * _moveSpeed;
            //MoveAndSlide();
        }
        private void ExitPreInteract()
        {
            Input.SetCustomMouseCursor(null, Input.CursorShape.Arrow);
        }
        private float _curTargetRangeSq = 100 * 100;
        private void RefreshCurTargetEntity()
        {
            if (_curTargetEntity != null && IsInstanceValid((Node2D)_curTargetEntity))
            {
                //todo 后面可以有什么组件就展示什么组件的信息 : 若有showHpComp, 就展示血条
                if (_curTargetEntity.Core.TryGetComponent<OutlineComponent>(out var outlineComp))
                {
                    outlineComp.Show(false);
                }
                _curTargetEntity = null;
            }

            Vector2 mousePos = GetGlobalMousePosition();
            if (GlobalPosition.DistanceSquaredTo(mousePos) > _curTargetRangeSq)
                return;
            var spaceState = GetWorld2D().DirectSpaceState;
            var query = new PhysicsPointQueryParameters2D();
            query.Position = mousePos;
            query.CollideWithAreas = true;
            query.CollideWithBodies = true;

            var results = spaceState.IntersectPoint(query);

            foreach (var result in results)
            {
                if (result["collider"].As<Node2D>() is IEntity entity && IsInstanceValid((Node2D)entity))
                {
                    _curTargetEntity = entity;
                    if (_curTargetEntity.Core.TryGetComponent<OutlineComponent>(out var outlineComp))
                    {
                        outlineComp.Show(true);
                    }
                    return;
                }
            }

        }
        #endregion

        #region Interact
        //private ITargetable? _curInteractingNode = null;
        private void EnterInteract()
        {
            _animSprite.Play("Action");

            //Tween tween = CreateTween();
            //tween.TweenInterval(2.0f);
            //tween.TweenCallback(Callable.From(() =>
            //{
            //    ChangeState(PlayerState.Idle);
            //    return;
            //}));
            //ResetAnim();
            //Input.SetCustomMouseCursor(_interactIconTexture, Input.CursorShape.Arrow, _aimIconTexture.GetSize() / 2);
        }
        private void UpdateInteract(float delta)
        {
            if (_animSprite.IsPlaying() == false)
            {
                //真正触发交互的地方

                ChangeState(PlayerState.Idle);
                return;
            }

            //if (Core.GetComponent<HpComponent>().CurValue <= 0)
            //{
            //    ChangeState(PlayerState.Death);
            //    return;
            //}
            //if (Input.IsActionJustReleased("Atk"))
            //{
            //    ChangeState(PlayerState.Idle);
            //    return;
            //}

            //if (_animSprite.Frame == 2)
            //{

            //}

            //RefreshFaceDir();
            //CheckTarget();
            //if (Input.IsActionJustReleased("Interact"))
            //{
            //    if (_curTarget == null || _curTarget.IsVaild())
            //    {
            //        ChangeState(PlayerState.Idle);
            //        return;
            //    }

            //    TriggerScreenShake(1);//震屏       
            //    if (_curTarget is BuildingCraft)
            //    {
            //        CraftView buildingCraftView = _craftViewPs.Instantiate<CraftView>();
            //        buildingCraftView.Init(CraftType.Building);
            //        _leftViewRootControl.AddChild(buildingCraftView);
            //        _curLeftView = buildingCraftView;
            //        ChangeState(PlayerState.BagUI);
            //        return;
            //    }
            //    else if (_curTarget is ToolCraft)
            //    {
            //        CraftView toolCraftView = _craftViewPs.Instantiate<CraftView>();
            //        toolCraftView.Init(CraftType.Tool);
            //        _leftViewRootControl.AddChild(toolCraftView);
            //        _curLeftView = toolCraftView;
            //        ChangeState(PlayerState.BagUI);
            //        return;
            //    }
            //    else if (_curTarget is ArmorCraft)
            //    {
            //        CraftView armorCraftView = _craftViewPs.Instantiate<CraftView>();
            //        armorCraftView.Init(CraftType.Armor);
            //        _leftViewRootControl.AddChild(armorCraftView);
            //        _curLeftView = armorCraftView;
            //        ChangeState(PlayerState.BagUI);
            //        return;
            //    }
            //    else
            //    {
            //        _curTarget.Interact();
            //    }
            //    ChangeState(PlayerState.Idle);
            //    return;

            //    //后期或许可以改成, 先根据手持物+交互目标,再进入动画, 动画结束再执行交互逻辑 
            //    //_animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            //    //_animTween.Parallel().TweenProperty(_animRootNode, "scale", new Vector2(0.8f, 0.8f), 0.05f);//出手动画
            //    //_animTween.TweenCallback(Callable.From(() =>
            //    //{


            //    //    //if (_curTarget is TreeGrow treeGrow && FastBarInventory.ItemInstanceList[CurFastBarIndex] != null)
            //    //    //{
            //    //    //    treeGrow.Watering(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type);
            //    //    //}
            //    //}));

            //    //_animTween.Parallel().TweenProperty(_animRootNode, "scale", new Vector2(1f, 1f), 0.1f);
            //    //_animTween.Finished += () =>
            //    //{
            //    //    //交互逻辑 : 由手持物+交互目标决定具体交互逻辑(如食物+小猫 = 投喂)


            //    //};
            //}

            //Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            //if (input != Vector2.Zero)
            //{
            //    //if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
            //    //    Velocity = input * _moveSpeed / 8;
            //    //else
            //    //    Velocity = input * _moveSpeed / 2;
            //    Velocity = input * _moveSpeed;
            //    MoveAndSlide();
            //}
        }
        private void ExitInteract()
        {
            Input.SetCustomMouseCursor(null, Input.CursorShape.Arrow);
            if (_curTarget != null && _curTarget.IsVaild())
            {
                _curTarget.ShowOutline(false);
                _curTarget = null;
            }
        }
        #endregion

        #region Build
        //[Export] private PackedScene _buildingPreviewPs = null!;
        //private BuildingPreview? _curBuildingPreview;
        private void EnterBuild()
        {
            _animSprite.Play("Idle");

            //_curBuildingPreview = _buildingPreviewPs.Instantiate<BuildingPreview>();
            //GetTree().CurrentScene.AddChild(_curBuildingPreview);
            //if (InventoryManager.GetCurItemType(_curFastBarIndex) is not ItemType curItemType)
            //    return;
            //_curBuildingPreview.Init(curItemType, GlobalPosition);
            //foreach (IQiRangeable qiRangeable in GameManager.Instance.IQiRangeableList)
            //{
            //    qiRangeable.ShowQiRange(true);
            //}
        }
        private void UpdateBuild(float delta)
        {
            if (Core.GetComponent<HpComponent>().CurHp <= 0)
            {
                foreach (IQiRangeable qiRangeable in GameManager.Instance.IQiRangeableList)
                {
                    qiRangeable.ShowQiRange(false);
                }
                ChangeState(PlayerState.Death);
                return;
            }
            if (Input.IsActionJustPressed("Interact"))
            {
                ChangeState(PlayerState.Idle);
                return;
            }


            RefreshFaceDir();

            //Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            //if (input != Vector2.Zero)
            //    _curMoveDir = input;
            //if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
            //    Velocity = input * _moveSpeed / 4;
            //else
            //    Velocity = input * _moveSpeed;
            //MoveAndSlide();

            //Vector2 mousePos = GetGlobalMousePosition();
            //_curBuildingPreview?.RefreshPosition(mousePos);

            //if (Input.IsActionJustReleased("Atk"))
            //{
            //    bool? isBuild = _curBuildingPreview?.Build(mousePos);            //    if (isBuild == true)
            //    {
            //        //InventoryManager.FastBarInventory.RemoveItemByIndex(_curFastBarIndex, 1);
            //    }
            //    ChangeState(PlayerState.Idle);
            //    return;
            //}

        }
        private void ExitBuild()
        {
            //_curBuildingPreview?.QueueFree();
            //foreach (IQiRangeable qiRangeable in GameManager.Instance.IQiRangeableList)
            //{
            //    qiRangeable.ShowQiRange(false);
            //}
        }
        #endregion

        #region Comsume
        private float _comsumeDuration = 2;
        private float _comsumeTimer = 0;
        private void EnterComsume()
        {
            _animSprite.Play("Action");
            //ResetAnim();
            //_animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            //_animTween.TweenProperty(_animRootNode, "scale", new Vector2(1.5f, 0.8f), 0.2f);
            //_animTween.TweenProperty(_animRootNode, "scale", new Vector2(1.0f, 1.0f), 0.2f);
            //_comsumeTimer = 0;
            //_atkLongPressTimer = 0;
            //_isAtkLongPressTimerValid = false;
        }
        private void UpdateComsume(float delta)
        {
            if (Core.GetComponent<HpComponent>().CurHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }
            if (Input.IsActionJustPressed("Interact"))
            {
                ChangeState(PlayerState.Idle);
                return;
            }
            if (_animSprite.IsPlaying() == false)
            {
                ComsumeItem();
                ChangeState(PlayerState.Idle);
                return;
            }
        }
        private void ExitComsume()
        {

        }
        #endregion

        #region Aim
        private void EnterAim()
        {
            _animSprite.Play("Idle");
            //ResetAnim();
            //_animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            //_animTween.TweenProperty(_animRootNode, "scale", new Vector2(1.5f, 0.8f), 0.2f);
            //_animTween.TweenProperty(_animRootNode, "scale", new Vector2(1.0f, 1.0f), 0.2f);
            Input.SetCustomMouseCursor(_aimIconTexture, Input.CursorShape.Arrow, _aimIconTexture.GetSize() / 2);
            //_isAtkLongPressTimerValid = false;
        }
        private void UpdateAim(float delta)
        {
            if (Core.GetComponent<HpComponent>().CurHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }

            if (Input.IsActionJustReleased("Atk"))
            {
                if (Core.GetComponent<InventoryComponent>().IsCurItemTypeExist(CurFastBarIndex) == false)
                {
                    ChangeState(PlayerState.Idle);
                    return;
                }
                switch (Core.GetComponent<InventoryComponent>().GetCurItemType(CurFastBarIndex))
                {
                    case ItemType.WoodSword:
                    case ItemType.IronSword:
                    case ItemType.GoldSword:
                    case ItemType.JadeSword:
                    case ItemType.WoodAxe:
                        ChangeState(PlayerState.Atk);
                        return;
                }
                //ChangeState(PlayerState.Idle);
                //return;
                //ItemType curItemType = FastBarInventory.ItemInstanceList[CurFastBarIndex].Type;
                //switch (curItemType)
                //{
                //    case ItemType.WoodBow:
                //    case ItemType.IronBow:
                //    case ItemType.GoldBow:
                //    case ItemType.JadeBow:
                //        //尝试扣除一个弓箭
                //        int removeCount = FastBarInventory.RemoveItemByType(ItemType.Arrow, 1);//先在快捷栏里扣
                //        if (removeCount != 1)
                //        {
                //            removeCount = BagInventory.RemoveItemByType(ItemType.Arrow, 1);//若快捷栏没有, 去背包里扣
                //        }
                //        if (removeCount == 1)
                //        {
                //            //发射弓箭, 
                //            Arrow arrow = _arrowPs.Instantiate<Arrow>();
                //            GetTree().CurrentScene.AddChild(arrow);
                //            arrow.Init(this, GlobalPosition, GetGlobalMousePosition(), _atk);
                //        }
                //        break;
                //    case ItemType.Fireball:

                //        Fireball fireball = _fireballPs.Instantiate<Fireball>();
                //        GetTree().CurrentScene.AddChild(fireball);
                //        fireball.Init(this, GlobalPosition, GetGlobalMousePosition(), _atk);

                //        GetTree().CreateTimer(0.1).Timeout += () =>
                //        {
                //            Fireball fireball = _fireballPs.Instantiate<Fireball>();
                //            GetTree().CurrentScene.AddChild(fireball);
                //            fireball.Init(this, GlobalPosition, GetGlobalMousePosition(), _atk);
                //            GetTree().CreateTimer(0.1).Timeout += () =>
                //            {
                //                Fireball fireball = _fireballPs.Instantiate<Fireball>();
                //                GetTree().CurrentScene.AddChild(fireball);
                //                fireball.Init(this, GlobalPosition, GetGlobalMousePosition(), _atk);
                //            };
                //        };
                //        break;
                //    case ItemType.WoodRod:
                //    case ItemType.IronRod:
                //    case ItemType.GoldRod:
                //    case ItemType.JadeRod:
                //        _atkLongPressTimer = 0;
                //        _isAtkLongPressTimerValid = true;
                //        ChangeState(PlayerState.Fishing);
                //        return;
                //}
                //_atkLongPressTimer = 0;
                ////_isAtkLongPressTimerValid = true;
                //ChangeState(PlayerState.Idle);
                //return;
            }
            if (Input.IsActionJustPressed("Interact"))
            {
                ChangeState(PlayerState.Idle);
                return;
            }

            RefreshFaceDir();

            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input != Vector2.Zero)
                _curMoveDir = input;
            if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
                Velocity = input * _moveSpeed / 4;
            else
                Velocity = input * _moveSpeed;
            MoveAndSlide();
        }
        private void ExitAim()
        {
            Input.SetCustomMouseCursor(null, Input.CursorShape.Arrow);
        }
        #endregion

        #region Fishing
        //private FishingFloat? fishingFloat;
        private void EnterFishing()
        {
            _animSprite.Play("Idle");
            //ResetAnim();
            //_animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            //_animTween.TweenProperty(_animRootNode, "scale", new Vector2(1.1f, 0.9f), 0.5f);
            //_animTween.TweenProperty(_animRootNode, "scale", new Vector2(1.0f, 1.0f), 0.5f);

            Vector2 mousePos = GetGlobalMousePosition();
            //if (GameManager.Instance.ChunkManager.GetTileType(mousePos) != TileType.Water)
            //{
            //    ChangeState(PlayerState.Idle);
            //    return;
            //}

            //fishingFloat = _fishingFloatPs.Instantiate<FishingFloat>();
            //GetTree().CurrentScene.AddChild(fishingFloat);
            //fishingFloat.Init(_handSprite.GlobalPosition, mousePos);
        }
        private void UpdateFishing(float delta)
        {
            //if (Core.GetComponent<HpComponent>().CurHp <= 0)
            //{
            //    fishingFloat?.QueueFree();
            //    ChangeState(PlayerState.Death);
            //    return;
            //}

            //if (Input.IsActionJustPressed("Interact"))
            //{
            //    fishingFloat?.QueueFree();
            //    ChangeState(PlayerState.Idle);
            //    return;
            //}
        }
        private void ExitFishing()
        {

        }
        #endregion

        #region Death
        private void EnterDeath()
        {
            CollisionLayer = 0;
            CollisionMask = 0;


            _animSprite.Play();
            //todo :
            //1. 播放死亡动画
            Tween _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            _animTween.TweenProperty(_animSprite, "scale", new Vector2(0.01f, 0.01f), 1f);
            _animTween.Finished += () =>
            {
                //2. 爆装备
                // 遍历快捷栏和背包, 把每个物品重新包装回dropItem, 在半径内随机生成, 然后调init
                //for (int i = 0; i < FastBarInventory.ItemInstanceList.Count; i++)
                //{
                //    if (FastBarInventory.ItemInstanceList[i] == null)
                //        continue;

                //    ItemInstance itemInstance = FastBarInventory.ItemInstanceList[i];
                //    DropItem dropItem = GameManager.Instance.DropItemPs.Instantiate<DropItem>();
                //    float randomAngle = (float)GD.RandRange(0, Mathf.Tau);
                //    float randomRadius = Mathf.Sqrt((float)GD.Randf()) * 30f;
                //    Vector2 randomOffset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomRadius;
                //    Vector2 randomPosition = GlobalPosition + randomOffset;
                //    GetTree().CurrentScene.AddChild(dropItem);
                //    dropItem.Init(itemInstance, randomPosition);
                //    dropItem.ApplyForce();
                //}
                //for (int i = 0; i < BagInventory.ItemInstanceList.Count; i++)
                //{
                //    if (BagInventory.ItemInstanceList[i] == null)
                //        continue;

                //    ItemInstance itemInstance = BagInventory.ItemInstanceList[i];
                //DropItem dropItem = GameManager.Instance.DropItemPs.Instantiate<DropItem>();
                //float randomAngle = (float)GD.RandRange(0, Mathf.Tau);
                //float randomRadius = Mathf.Sqrt((float)GD.Randf()) * 30f;
                //Vector2 randomOffset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomRadius;
                //Vector2 randomPosition = GlobalPosition + randomOffset;
                //GetTree().CurrentScene.AddChild(dropItem);
                //dropItem.Init(itemInstance, randomPosition);
                //dropItem.ApplyForce();
                //}

                //3. 等待1s
                //GetTree().CreateTimer(1).Timeout += () =>
                //{
                //    //弹出菜单 : 退出到主菜单 or 复活
                //    _deathView.Visible = true;
                //};
            };
        }
        private void UpdateDeath(float delta)
        {
            if (Core.GetComponent<HpComponent>().CurHp > 0)
            {
                ChangeState(PlayerState.Idle);
                return;
            }
        }
        private void ExitDeath()
        {

        }
        #endregion

        #region BagUI
        private Control? _curLeftView;
        private LeftViewType _curLeftViewType = LeftViewType.None;
        private void EnterBagUI()
        {
            _animSprite.Play("Idle");

            //Tween tween = CreateTween().SetParallel(true);
            //tween.TweenProperty(_camera, "zoom", new Vector2(10, 10), 0.1f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
            //tween.TweenProperty(_camera, "offset", new Vector2(65, -10), 0.1f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

            GameManager.Instance.UIManager.InventoryManagerView.Visible = true;
            GameManager.Instance.UIManager.FastBarView.Visible = false;
            if (_curLeftView == null)
                GameManager.Instance.UIManager.CharacterView.Visible = true;
            else
                GameManager.Instance.UIManager.CharacterView.Visible = false;
        }
        private void UpdateBagUI(float delta)
        {
            if (Input.IsActionJustPressed("Bag"))
            {
                ChangeState(PlayerState.Idle);
                return;
            }
        }
        private void ExitBagUI()
        {
            //Tween tween = CreateTween().SetParallel(true);
            //tween.TweenProperty(_camera, "zoom", new Vector2(1, 1), 0.1f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
            //tween.TweenProperty(_camera, "offset", new Vector2(0, 0), 0.1f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

            GameManager.Instance.UIManager.InventoryManagerView.Visible = false;
            GameManager.Instance.UIManager.FastBarView.Visible = true;
            if (_curLeftView == null)
            {
                GameManager.Instance.UIManager.CharacterView.Visible = false;
            }
            else
            {
                _curLeftView.QueueFree();
                _curLeftView = null;
            }
        }
        #endregion





















































        // 用鼠标检测目标, 当前范围检测, 触发攻击/交互时再判断距离
        private ITargetable? _curTarget = null;
        private void CheckTarget()
        {
            if (_curTarget != null && _curTarget.IsVaild())
            {
                _curTarget.ShowOutline(false);
                _curTarget = null;
            }

            Vector2 mousePos = GetGlobalMousePosition();
            if (GlobalPosition.DistanceSquaredTo(mousePos) > _curTargetRangeSq)
                return;
            var spaceState = GetWorld2D().DirectSpaceState;
            var query = new PhysicsPointQueryParameters2D();
            query.Position = mousePos;
            query.CollideWithAreas = true;
            query.CollideWithBodies = true;

            var results = spaceState.IntersectPoint(query);

            foreach (var result in results)
            {
                if (result["collider"].As<Node2D>() is ITargetable target && target.IsVaild())
                {
                    _curTarget = target;
                    _curTarget.ShowOutline(true);
                    return;
                }
            }


        }

        private Node2D? _curEquipmentNode = null;
        private void ChangeCurFastBarIndex(bool isNext)
        {
            if (isNext)
            {
                if (CurFastBarIndex >= Core.GetComponent<InventoryComponent>().FastBarInventory.SlotList.Count - 1)
                    return;
                CurFastBarIndex++;
            }
            else
            {
                if (CurFastBarIndex <= 0)
                    return;
                CurFastBarIndex--;
            }
            OnCurFastBarIndexChanged?.Invoke();
            RefreshHandNode();
        }
        private void RefreshHandNode()
        {
            _handSprite.Texture = null;
            _handSprite.Position = new Vector2(0f, 0f);
            _handSprite.RotationDegrees = 0;
            _handSprite.Scale = new Vector2(1f, 1f);

            if (Core.GetComponent<InventoryComponent>().IsCurItemTypeExist(CurFastBarIndex) == false)
                return;
            ItemType itemType = Core.GetComponent<InventoryComponent>().GetCurItemType(CurFastBarIndex);


            ItemData itemData = ItemDataManager.Instance.GetData((ItemType)itemType);

            _handSprite.Texture = GD.Load<Texture2D>(itemData.IconPath);
            if (itemData.CanBuild)
            {
                //建筑物, 缩小
                _handSprite.Scale = new Vector2(0.2f, 0.2f);
            }
            if (itemType == ItemType.WoodSword || itemType == ItemType.WoodAxe || itemType == ItemType.WoodPickaxe || itemType == ItemType.WoodRod ||
                itemType == ItemType.IronSword || itemType == ItemType.IronAxe || itemType == ItemType.IronPickaxe || itemType == ItemType.IronRod ||
                itemType == ItemType.GoldSword || itemType == ItemType.GoldAxe || itemType == ItemType.GoldPickaxe || itemType == ItemType.GoldRod ||
                itemType == ItemType.JadeSword || itemType == ItemType.JadeAxe || itemType == ItemType.JadePickaxe || itemType == ItemType.JadeRod
                )
            {
                _handSprite.Position = new Vector2(0f, -7f);
                _handSprite.RotationDegrees = -45;
            }
            if (itemType == ItemType.WoodBow || itemType == ItemType.IronBow || itemType == ItemType.GoldBow || itemType == ItemType.JadeBow || itemType == ItemType.Fireball)
            {
                _handSprite.RotationDegrees = 45;
            }
        }
        private void RefreshFaceDir()
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            Vector2 direction = mousePosition - _handRootNode.GlobalPosition;
            _handRootNode.GlobalRotation = direction.Angle();

            Vector2 curFaceDir = (GetGlobalMousePosition() - GlobalPosition).Normalized();
            if (curFaceDir.X < 0)
                _animSprite.Scale = new Vector2(-1, 1);
            else if (curFaceDir.X > 0)
                _animSprite.Scale = new Vector2(1, 1);
        }

        private Tween? _shakeTween;
        public void TriggerScreenShake(float amount)
        {
            if (_shakeTween != null && _shakeTween.IsValid())
            {
                _shakeTween.Kill();
            }
            _shakeTween = CreateTween();
            _shakeTween.SetParallel(false);
            int shakeCount = 6;          // 震动来回摆动的次数
            float duration = 0.05f;      // 每次摆动花费的时间（秒）
            float currentAmount = amount; // 当前强度的副本，用于逐步衰减
            for (int i = 0; i < shakeCount; i++)
            {
                float randomX = (GD.Randf() * 2.0f - 1.0f) * currentAmount;
                float randomY = (GD.Randf() * 2.0f - 1.0f) * currentAmount;
                Vector2 targetOffset = new Vector2(randomX, randomY);
                _shakeTween.TweenProperty(_camera, "offset", targetOffset, duration).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                currentAmount *= 0.7f;
            }
            _shakeTween.TweenProperty(_camera, "offset", Vector2.Zero, duration);
        }

        //复活
        public void Revive()
        {
            GlobalPosition = Core.GetComponent<StartPositionComponent>().StartPositon;
            Core.GetComponent<RealmComponent>().Reset();
            Core.GetComponent<HpComponent>().Reset();
            Core.GetComponent<QiComponent>().Reset();
            Core.GetComponent<ExpComponent>().Reset();
            CollisionLayer = 1;
            CollisionMask = 1;
        }

        private float _mpConsumeDuration = 0.1f;//每0.1秒 -consumeValue
        private float _mpConsumeTimer = 0;
        private void ConsumeDuration(float delta, float consumeValue)
        {
            //_mpConsumeTimer += delta;
            //if (_mpConsumeTimer < _mpConsumeDuration)
            //    return;
            //if (_curMp - consumeValue > 0)
            //{
            //    SetCurMp(_curMp - consumeValue);
            //}
            //else
            //{
            //    SetCurMp(0);
            //}
            //_mpConsumeTimer = 0;
        }

        private void ComsumeItem()
        {
            //var itemInstance = FastBarInventory.ItemInstanceList[CurFastBarIndex];
            //if (itemInstance == null)
            //    return;

            //ItemData itemData = ItemDataManager.Instance.GetItemData(itemInstance.Type);

            //if (itemData.HpBonus > 0)
            //    GetHp(itemData.HpBonus);
            //if (itemData.MpBonus > 0)
            //    GetMp(itemData.MpBonus);

            //int remainCount = FastBarInventory.RemoveItemByIndex(CurFastBarIndex, 1);
            //if (remainCount == 0)
            //    RefreshHandNode();
        }
    }
}


