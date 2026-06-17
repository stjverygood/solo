using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.System.InventorySystem;
using Solo.Scripts.System.ItemSystem;
using Solo.Scripts.System.SaveSystem;

namespace Solo.Scripts.Entities.Players
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Dash,
        //PreAtk,//预攻击, 选目标
        Atk,//攻击
        //PreInteract,//预交互, 选目标
        Interact,//交互
        Build,//建造状态
        Death,
        BagUI,
    }

    public partial class Player : CharacterBody2D, ITargetable
    {
        public UnitType Type = UnitType.Player;
        private Vector2 _curDir = Vector2.Right;
        public PlayerState CurState;
        public float moveSpeed = 50;
        [Export] public Node2D SpriteRoot;//附带上身体之外的交互点, 比如后面拍建筑的定位点, 用于控制功能交互的
        [Export] public Node2D BodyRoot;//仅仅是身体的根节点, 用于控制动画
        [Export] public Area2D ViewArea;//视角内
        [Export] private Camera2D _camera;
        public float Atk = 10;

        private float _meleeAtkRange = 30;
        private float _rangeAtkRange = 50;
        private float _curAtkRange;//根据itemType动态切换攻击距离
        private float _curAtkRangeSq;

        private float _interactRange = 30;
        private float _interactRangeSq;

        private float _curTargetRange = 100;//手长, 攻击和交互都统一用这个距离, 远程itemtype能加这个范围
        private float _curTargetRangeSq;

        public int ResCapacity = 1000;//资源存储上限
        private Tween _animTween; // 用于管理当前动画

        public Inventory BagInventory = new Inventory();//背包
        public Inventory FastBarInventory = new Inventory();//快捷栏


        [Export] private SelfView _selfView;
        [Export] private InventoryView _bagInventoryView;
        [Export] private InventoryView _fastBarInventoryView;


        public override void _Ready()
        {
            GD.Print("Player Ready~~~");
            GameManager.Instance.Player = this;

            GlobalPosition = new Vector2(SaveManager.Instance.CurSaveData.PlayerPosX, SaveManager.Instance.CurSaveData.PlayerPosY);
            BagInventory.GuidStr = SaveManager.Instance.CurSaveData.BagInventoryGuidStr;
            BagInventory.ItemInstanceList = SaveManager.Instance.CurSaveData.BagInventoryList;
            FastBarInventory.GuidStr = SaveManager.Instance.CurSaveData.FastBarInventoryGuidStr;
            FastBarInventory.ItemInstanceList = SaveManager.Instance.CurSaveData.FastBarInventoryList;
            CurFastBarIndex = SaveManager.Instance.CurSaveData.FastBarIndex;


            _curAtkRange = _meleeAtkRange;//todo : 根据itemdata的israngeitem来决定攻击范围
            _curAtkRangeSq = _curAtkRange * _curAtkRange;
            _interactRangeSq = _interactRange * _interactRange;

            _curTargetRangeSq = _curTargetRange * _curTargetRange;

            //ViewArea.AreaEntered += ViewArea_AreaEntered;
            //ViewArea.AreaExited += ViewArea_AreaExited;
            //ViewArea.BodyEntered += ViewArea_BodyEntered;
            //ViewArea.BodyExited += ViewArea_BodyExited;

            if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
            {
                ChangeState(PlayerState.Build);
            }
            else
            {
                ChangeState(PlayerState.Idle);
            }

            _selfView.BasicCraftView.Init();
            _selfView.Visible = false;
            _bagInventoryView.Init(BagInventory);
            _fastBarInventoryView.Init(FastBarInventory);
            _bagInventoryView.Visible = false;
            _fastBarInventoryView.SetSelected(CurFastBarIndex, true);
            RefreshHandNode();
        }

        public override void _PhysicsProcess(double delta)
        {
            UpdateState((float)delta);
            //GD.Print("_curTarget : " + _curTarget);
            //GD.Print($"CurState : {CurState}");
            //GD.Print($"GlobalPosition : {GlobalPosition}");
        }

        private void ChangeState(PlayerState newState)
        {
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
                //case PlayerState.PreAtk:
                //    EnterPreAtk();
                //    break;
                case PlayerState.Atk:
                    EnterAtk();
                    break;
                //case PlayerState.PreInteract:
                //    EnterPreInteract();
                //    break;
                case PlayerState.Interact:
                    EnterInteract();
                    break;
                case PlayerState.Build:
                    EnterBuild();
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
                //case PlayerState.PreAtk:
                //    UpdatePreAtk(delta);
                //    break;
                case PlayerState.Atk:
                    UpdateAtk(delta);
                    break;
                //case PlayerState.PreInteract:
                //    UpdatePreInteract(delta);
                //    break;
                case PlayerState.Interact:
                    UpdateInteract(delta);
                    break;
                case PlayerState.Build:
                    UpdateBuild(delta);
                    break;
                case PlayerState.Death:
                    UpdateDeath(delta);
                    break;
                case PlayerState.BagUI:
                    UpdateBagUI(delta);
                    break;
            }
        }

        #region Idle
        private void EnterIdle()
        {
            ResetAnim();
            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            _animTween.TweenProperty(BodyRoot, "scale", new Vector2(1.2f, 0.8f), 0.5f);
            _animTween.TweenProperty(BodyRoot, "scale", new Vector2(1.0f, 1.0f), 0.5f);
        }
        private void UpdateIdle(float delta)
        {
            if (Input.IsActionJustPressed("Pre"))
            {
                ChangeCurFastBarIndex(false);
                //if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                //{
                //    ChangeState(PlayerState.Build);
                //    return;
                //}
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
                //if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                //{
                //    ChangeState(PlayerState.Build);
                //    return;
                //}
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

            CheckTarget();
            //RefreshNearestTarget();//每帧刷新最近的target
            if (Input.IsActionJustPressed("Atk"))
            {
                //根据物品, 若是buildingItem, 转到建筑模式
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
                else
                {
                    ChangeState(PlayerState.Atk);
                    return;
                }
            }
            if (Input.IsActionJustPressed("Interact"))
            {
                //根据物品, 若是buildingItem, 转到建筑模式
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
                else
                {
                    ChangeState(PlayerState.Interact);
                    return;
                }
            }

            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input != Vector2.Zero)
            {
                ChangeState(PlayerState.Walk);
                return;
            }

            //CheckInteractTarget();
            //if (Input.IsActionJustPressed("Interact") && _curInteractTargetNode != null)
            //{
            //    if (_curInteractTargetNode is DropItem dropItem)
            //        dropItem.Pickup();
            //}
        }
        #endregion

        #region Walk
        private void EnterWalk()
        {
            ResetAnim();
            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            _animTween.TweenProperty(BodyRoot, "skew", 0.1f, 0.3f);// 走动效果：左右晃动或轻微拉伸
            _animTween.TweenProperty(BodyRoot, "skew", -0.1f, 0.3f);
        }
        private void UpdateWalk(float delta)
        {
            if (Input.IsActionJustPressed("Pre"))
            {
                ChangeCurFastBarIndex(false);
                //if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                //{
                //    ChangeState(PlayerState.Build);
                //    return;
                //}
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
                //if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                //{
                //    ChangeState(PlayerState.Build);
                //    return;
                //}
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

            CheckTarget();
            if (Input.IsActionJustPressed("Atk"))
            {
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
                else
                {
                    ChangeState(PlayerState.Atk);
                    return;
                }
            }
            if (Input.IsActionJustPressed("Interact"))
            {
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
                else
                {
                    ChangeState(PlayerState.Interact);
                    return;
                }
            }
            //RefreshNearestTarget();//每帧刷新最近的target
            //if (Input.IsActionJustPressed("Atk"))
            //{
            //    ChangeState(PlayerState.PreAtk);
            //    return;
            //}
            //if (Input.IsActionJustPressed("Interact"))
            //{
            //    ChangeState(PlayerState.PreInteract);
            //    return;
            //}

            //CheckAtkTarget();
            //if (Input.IsActionJustPressed("Atk"))
            //{
            //    ChangeState(PlayerState.Atk);
            //    return;
            //}
            //CheckInteractTarget();
            //if (Input.IsActionJustPressed("Interact") && _curInteractTargetNode != null)
            //{
            //    if (_curInteractTargetNode is DropItem dropItem)
            //        dropItem.Pickup();
            //}

            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input == Vector2.Zero)
            {
                ChangeState(PlayerState.Idle);
                return;
            }
            _curDir = input;
            if (input.X < 0)
                SpriteRoot.Scale = new Vector2(-1, 1);
            else if (input.X > 0)
                SpriteRoot.Scale = new Vector2(1, 1);
            if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
                Velocity = input * moveSpeed / 4;
            else
                Velocity = input * moveSpeed;
            MoveAndSlide();
        }
        #endregion

        #region Run
        private void EnterRun()
        {
            ResetAnim();
            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            _animTween.TweenProperty(BodyRoot, "skew", 0.1f, 0.1f);// 走动效果：左右晃动或轻微拉伸
            _animTween.TweenProperty(BodyRoot, "skew", -0.1f, 0.1f);
        }
        private void UpdateRun(float delta)
        {
            if (Input.IsActionJustPressed("Pre"))
            {
                ChangeCurFastBarIndex(false);
                //if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                //{
                //    ChangeState(PlayerState.Build);
                //    return;
                //}
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
                //if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                //{
                //    ChangeState(PlayerState.Build);
                //    return;
                //}
            }

            CheckTarget();
            if (Input.IsActionJustPressed("Atk"))
            {
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
                else
                {
                    ChangeState(PlayerState.Atk);
                    return;
                }
            }
            if (Input.IsActionJustPressed("Interact"))
            {
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
                else
                {
                    ChangeState(PlayerState.Interact);
                    return;
                }
            }
            //RefreshNearestTarget();//每帧刷新最近的target
            //if (Input.IsActionJustPressed("Atk"))
            //{
            //    ChangeState(PlayerState.PreAtk);
            //    return;
            //}
            //if (Input.IsActionJustPressed("Interact"))
            //{
            //    ChangeState(PlayerState.PreInteract);
            //    return;
            //}

            //CheckInteractTarget();
            //if (Input.IsActionJustPressed("Interact") && _curInteractTargetNode != null)
            //{
            //    if (_curInteractTargetNode is DropItem dropItem)
            //        dropItem.Pickup();
            //}
            //CheckAtkTarget();
            //if (Input.IsActionJustPressed("Atk"))
            //{
            //    ChangeState(PlayerState.Atk);
            //    return;
            //}

            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input == Vector2.Zero || Input.IsActionPressed("Dash") == false)
            {
                ChangeState(PlayerState.Idle);
                return;
            }
            _curDir = input;
            if (input.X < 0)
                SpriteRoot.Scale = new Vector2(-1, 1);
            else if (input.X > 0)
                SpriteRoot.Scale = new Vector2(1, 1);
            //TouchArea.Rotation = input.Angle();
            if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
                Velocity = input * moveSpeed / 2;
            else
                Velocity = input * moveSpeed * 2;
            MoveAndSlide();
        }
        #endregion

        #region Dash
        private float _dashTimer;
        private float _dashDuration = 0.2f;
        private float _dashSpeed = 200;
        private void EnterDash()
        {
            ResetAnim();
            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            _animTween.TweenProperty(BodyRoot, "skew", 0.1f, 0.3f);// 走动效果：左右晃动或轻微拉伸
            _animTween.TweenProperty(BodyRoot, "skew", -0.1f, 0.3f);
            _dashTimer = 0;
        }
        private void UpdateDash(float delta)
        {
            //CheckInteractTarget();
            //if (Input.IsActionJustPressed("Interact") && _curInteractTargetNode != null)
            //{
            //    if (_curInteractTargetNode is DropItem dropItem)
            //        dropItem.Pickup();
            //}

            if (Input.IsActionJustPressed("Pre"))
            {
                ChangeCurFastBarIndex(false);
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
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

            Velocity = _curDir * _dashSpeed;
            MoveAndSlide();
        }
        #endregion

        #region Atk
        private void EnterAtk()
        {
            ResetAnim();

            //判断目标有效性, 超出攻击范围直接返回idle
            //if (GlobalPosition.DistanceSquaredTo(_atkTargetSortList[_atkTargetIndex].GetWorldPosition()) > _curAtkRange)
            //{
            //    ChangeState(PlayerState.Idle);
            //    return;
            //}

            if (_curTarget == null || _curTarget.IsVaild() == false || _curTarget.CanAtk() == false)
            {
                ChangeState(PlayerState.Idle);
                return;
            }

            if (_curTarget.GetWorldPosition().X - GlobalPosition.X < 0)
                SpriteRoot.Scale = new Vector2(-1, 1);
            else
                SpriteRoot.Scale = new Vector2(1, 1);

            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            _animTween.Parallel().TweenProperty(BodyRoot, "scale", new Vector2(0.8f, 0.8f), 0.05f);//出手动画
            _animTween.TweenProperty(_handNode, "rotation", 1.3f, 0.05f);
            _animTween.TweenCallback(Callable.From(() =>
            {
                TriggerScreenShake(5);//震屏
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).MaxDur != -1)//有工具耐久
                {
                    FastBarInventory.ItemInstanceList[CurFastBarIndex].CurDur--;
                    if (FastBarInventory.ItemInstanceList[CurFastBarIndex].CurDur <= 0)
                    {
                        FastBarInventory.RemoveItem(CurFastBarIndex);
                        RefreshHandNode();
                    }
                    _fastBarInventoryView.RefreshSlot(CurFastBarIndex);
                }
                _curTarget.TakeDamage(Atk, FastBarInventory.ItemInstanceList[CurFastBarIndex]?.Type);
            }));

            _animTween.Parallel().TweenProperty(BodyRoot, "scale", new Vector2(1f, 1f), 0.1f);
            _animTween.TweenProperty(_handNode, "rotation", 0, 0.1f);
            _animTween.Finished += () =>
            {
                ChangeState(PlayerState.Idle);
            };
        }
        private void UpdateAtk(float delta)
        {
            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input != Vector2.Zero)
            {
                if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
                    Velocity = input * moveSpeed / 8;
                else
                    Velocity = input * moveSpeed / 2;
                MoveAndSlide();
            }
        }
        #endregion

        #region Interact
        private void EnterInteract()
        {
            ResetAnim();

            if (_curTarget == null || _curTarget.IsVaild() == false || _curTarget.CanInteract() == false)
            {
                ChangeState(PlayerState.Idle);
                return;
            }

            if (_curTarget.GetWorldPosition().X - GlobalPosition.X < 0)
                SpriteRoot.Scale = new Vector2(-1, 1);
            else
                SpriteRoot.Scale = new Vector2(1, 1);


            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            _animTween.Parallel().TweenProperty(BodyRoot, "scale", new Vector2(0.8f, 0.8f), 0.05f);//出手动画
            _animTween.TweenCallback(Callable.From(() =>
            {
                TriggerScreenShake(1);//震屏
                _curTarget.Interact();
            }));

            _animTween.Parallel().TweenProperty(BodyRoot, "scale", new Vector2(1f, 1f), 0.1f);
            _animTween.Finished += () =>
            {
                ChangeState(PlayerState.Idle);
            };
        }
        private void UpdateInteract(float delta)
        {

        }
        #endregion

        #region Build
        [Export] private PackedScene _buildingPreviewPs;
        private BuildingPreview _curBuildingPreview;
        private void EnterBuild()
        {
            ResetAnim();
            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            _animTween.TweenProperty(BodyRoot, "skew", 0.1f, 0.3f);// 走动效果：左右晃动或轻微拉伸
            _animTween.TweenProperty(BodyRoot, "skew", -0.1f, 0.3f);
            _curBuildingPreview = _buildingPreviewPs.Instantiate<BuildingPreview>();
            GetTree().CurrentScene.AddChild(_curBuildingPreview);
            _curBuildingPreview.Init(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type, GlobalPosition);
        }
        private void UpdateBuild(float delta)
        {
            //if (Input.IsActionJustPressed("Pre"))
            //{
            //    ChangeCurFastBarIndex(false);
            //    if (FastBarInventory.ItemInstanceList[CurFastBarIndex] == null || ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding == false)
            //    {
            //        _curBuildingPreview.QueueFree();
            //        ChangeState(PlayerState.Idle);
            //        return;
            //    }
            //}
            //if (Input.IsActionJustPressed("Next"))
            //{
            //    ChangeCurFastBarIndex(true);
            //    if (FastBarInventory.ItemInstanceList[CurFastBarIndex] == null || ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding == false)
            //    {
            //        _curBuildingPreview.QueueFree();
            //        ChangeState(PlayerState.Idle);
            //        return;
            //    }
            //}

            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input != Vector2.Zero)
                _curDir = input;
            if (input.X < 0)
                SpriteRoot.Scale = new Vector2(-1, 1);
            else if (input.X > 0)
                SpriteRoot.Scale = new Vector2(1, 1);
            if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
                Velocity = input * moveSpeed / 4;
            else
                Velocity = input * moveSpeed;
            MoveAndSlide();

            Vector2 mousePos = GetGlobalMousePosition();
            _curBuildingPreview.RefreshPosition(mousePos);
            if (Input.IsActionJustPressed("Atk"))
            {
                bool isBuild = _curBuildingPreview.Build();                if (isBuild)
                {
                    int remainCount = FastBarInventory.RemoveItemByIndex(CurFastBarIndex, 1);
                    if (remainCount == 0)//count = 0, 空手, 并且切到Idle
                    {
                        _curBuildingPreview.QueueFree();
                        ChangeState(PlayerState.Idle);
                        return;
                    }
                }
            }
            if (Input.IsActionJustPressed("Interact"))
            {
                ChangeState(PlayerState.Idle);
                return;
            }
        }
        #endregion

        #region death
        private void EnterDeath()
        {
            ResetAnim();
        }
        private void UpdateDeath(float delta)
        {

        }
        #endregion

        #region bagUI
        private void EnterBagUI()
        {
            ResetAnim();
            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            _animTween.TweenProperty(BodyRoot, "scale", new Vector2(1.2f, 0.8f), 0.5f);
            _animTween.TweenProperty(BodyRoot, "scale", new Vector2(1.0f, 1.0f), 0.5f);
            _selfView.Visible = true;
            _bagInventoryView.Visible = true;
        }
        private void UpdateBagUI(float delta)
        {
            if (Input.IsActionJustPressed("Bag") || Input.IsActionJustPressed("Back"))
            {
                _selfView.Visible = false;
                _bagInventoryView.Visible = false;
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
                {
                    ChangeState(PlayerState.Build);
                    return;
                }
                else
                {
                    ChangeState(PlayerState.Idle);
                    return;
                }
            }
        }
        #endregion




        private void ResetAnim()
        {
            if (_animTween != null && _animTween.IsRunning())
                _animTween.Kill();
            BodyRoot.Scale = Vector2.One;
            BodyRoot.Skew = 0f;
            BodyRoot.Modulate = Colors.White;
            BodyRoot.Rotation = 0f;
            BodyRoot.Modulate = Colors.White;
        }


        //todo : !!!!!!!!!!!!!!!!!!!!!!!
        //统一用viewarea来检测, 增加IInteractable, 左键攻击, 右键交互
        //或则滚轮切目标? 然后把进入/退出建筑状态的条件改一下, 比如切到建筑, 然后按左键, 才会进入建筑状态
        //在建筑状态, 滚轮是切方向, 按左键拍建筑, 按右键退出, 输入左键要先判断是否为建筑物品了(已有isBuilding)
        //由于右键+滚轮不是很舒服, 所以添加tab键的功能, 和向下滚一样
        //右键交互逻辑和攻击逻辑一样, 长按进入交互选时才标记轮廓?
        //交互list, 攻击list
        //同一用一个nodelist, 不断把最近node轮廓标记, 左键攻击, 右键
        //交互, 长按进入切目标
        //

        //private ITargetable _curNearestInteractTarget;
        //private ITargetable _curNearestAtkTarget;
        //private List<ITargetable> _curInteractTargetList = new List<ITargetable>();//记录视野内的IInteractable
        //private List<ITargetable> _curAtkTargetList = new List<ITargetable>();//记录视野内的ICombatable

        //private void RefreshNearestTarget()//把最近的可攻击, 可交互目标标记出来, 可攻击轮廓, 可交互ui
        //{
        //    _curNearestAtkTarget?.ShowAtkTip(false);
        //    _curNearestInteractTarget?.ShowInteractTip(false);
        //    _curNearestAtkTarget = null;
        //    _curNearestInteractTarget = null;

        //    float minDistSq = float.MaxValue;
        //    foreach (ITargetable atkTarget in _curAtkTargetList)
        //    {
        //        float curDistSq = GlobalPosition.DistanceSquaredTo(atkTarget.GetWorldPosition());
        //        if (curDistSq > _curAtkRangeSq)
        //            continue;
        //        if (curDistSq > minDistSq)
        //            continue;
        //        minDistSq = curDistSq;
        //        _curNearestAtkTarget = atkTarget;
        //    }

        //    minDistSq = float.MaxValue;
        //    foreach (ITargetable interactable in _curInteractTargetList)
        //    {
        //        float curDistSq = GlobalPosition.DistanceSquaredTo(interactable.GetWorldPosition());
        //        if (curDistSq > _interactRangeSq)
        //            continue;
        //        if (curDistSq > minDistSq)
        //            continue;
        //        minDistSq = curDistSq;
        //        _curNearestInteractTarget = interactable;
        //    }

        //    _curNearestAtkTarget?.ShowAtkTip(true);
        //    _curNearestInteractTarget?.ShowInteractTip(true);
        //}

        //private void ViewArea_AreaEntered(Area2D area)
        //{
        //    if (area is ITargetable targetable)
        //    {
        //        if (targetable.CanInteract())
        //        {
        //            _curInteractTargetList.Add(targetable);
        //        }
        //        if (targetable.CanAtk())
        //        {
        //            _curAtkTargetList.Add(targetable);
        //        }
        //    }
        //}
        //private void ViewArea_AreaExited(Area2D area)
        //{
        //    if (area is ITargetable targetable)
        //    {
        //        if (targetable.CanInteract())
        //        {
        //            _curInteractTargetList.Remove(targetable);
        //        }
        //        if (targetable.CanAtk())
        //        {
        //            _curAtkTargetList.Remove(targetable);
        //        }
        //    }
        //}
        //private void ViewArea_BodyEntered(Node2D body)
        //{
        //    if (body is ITargetable targetable)
        //    {
        //        if (targetable == this) return;
        //        if (targetable.CanInteract())
        //        {
        //            _curInteractTargetList.Add(targetable);
        //        }
        //        if (targetable.CanAtk())
        //        {
        //            _curAtkTargetList.Add(targetable);
        //        }
        //    }
        //}
        //private void ViewArea_BodyExited(Node2D body)
        //{
        //    if (body is ITargetable targetable)
        //    {
        //        if (targetable.CanInteract())
        //        {
        //            _curInteractTargetList.Remove(targetable);
        //        }
        //        if (targetable.CanAtk())
        //        {
        //            _curAtkTargetList.Remove(targetable);
        //        }
        //    }
        //}







        // 用鼠标检测目标, 当前范围检测, 触发攻击/交互时再判断距离
        private ITargetable _curTarget = null;
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


        //private List<Node2D> _interactTargetNodeList = new List<Node2D>();
        //private Node2D _curInteractTargetNode;
        //private void InteractArea_AreaEntered(Area2D area)
        //{
        //    if (area is DropItem dropItem)
        //    {
        //        _interactTargetNodeList.Add(dropItem);
        //    }
        //}
        //private void InteractArea_AreaExited(Area2D area)
        //{
        //    if (area is DropItem dropItem)
        //    {
        //        _interactTargetNodeList.Remove(dropItem);
        //    }
        //}
        //private void InteractArea_BodyEntered(Node2D body)
        //{
        //    //if (body is DropItem dropItem)
        //    //{
        //    //    _interactTargetNodeList.Add(dropItem);
        //    //}
        //}
        //private void InteractArea_BodyExited(Node2D body)
        //{
        //    //if (body is DropItem dropItem)
        //    //{
        //    //    _interactTargetNodeList.Remove(dropItem);
        //    //}
        //}
        //private void CheckInteractTarget()
        //{
        //    Node2D oldInteractTargetNode = _curInteractTargetNode;
        //    Node2D newInteractTargetNode = GlobalHelper.GetNearestNode(GlobalPosition, _interactTargetNodeList);
        //    if (oldInteractTargetNode != newInteractTargetNode)// 1. 只有当最近的物体“发生改变”时，才处理开关逻辑
        //    {
        //        if (IsInstanceValid(oldInteractTargetNode) && oldInteractTargetNode is DropItem oldDropItem)// 2. 关掉旧目标的文本（如果旧目标还存在的话）
        //        {
        //            oldDropItem.ShowText(false);
        //        }
        //        if (IsInstanceValid(newInteractTargetNode) && newInteractTargetNode is DropItem newDropItem)// 3. 开启新目标的文本（如果新目标存在的话）
        //        {
        //            newDropItem.ShowText(true);
        //        }
        //        _curInteractTargetNode = newInteractTargetNode;// 4. 交接变量，完成记忆更新
        //    }
        //}



        //private void AtkArea_BodyEntered(Node2D body)
        //{
        //    if (body is ICombatable combat)//todo : enemy
        //    {
        //        _curCombatList.Add(building);
        //    }
        //}
        //private void AtkArea_BodyExited(Node2D body)
        //{
        //    if (body is Building building)//todo : enemy
        //    {
        //        _atkTargetBuildingList.Remove(building);
        //    }
        //}
        //private void CheckAtkUnitTarget()
        //{
        //    //先清除旧目标标记
        //    _curAtkTargetUnit?.ShowOutline(false);
        //    _curAtkTargetBuilding?.ShowOutline(false);
        //    Unit newUnit = GlobalHelper.GetNearestNode(GlobalPosition, _atkTargetUnitList) as Unit;
        //    _curAtkTargetUnit = newUnit;
        //    _curAtkTargetUnit.ShowOutline(true);
        //}
        //private void CheckAtkBuildingTarget()
        //{
        //    //先清除旧目标标记
        //    _curAtkTargetUnit?.ShowOutline(false);
        //    _curAtkTargetBuilding?.ShowOutline(false);

        //}
        //private void CheckAtkTarget()
        //{
        //    _curAtkTargetUnit?.ShowOutline(false);
        //    _curAtkTargetUnit = null;
        //    _curAtkTargetBuilding?.ShowOutline(false);
        //    _curAtkTargetBuilding = null;
        //    Unit newUnit = GlobalHelper.GetNearestNode(GlobalPosition, _atkTargetUnitList) as Unit;
        //    if (newUnit != null && IsInstanceValid(newUnit))
        //    {
        //        _curAtkTargetUnit = newUnit;
        //        _curAtkTargetUnit.ShowOutline(true);
        //    }
        //    else
        //    {
        //        Building newBuilding = GlobalHelper.GetNearestNode(GlobalPosition, _atkTargetBuildingList) as Building;
        //        if (newBuilding != null && IsInstanceValid(newBuilding))
        //        {
        //            _curAtkTargetBuilding = newBuilding;
        //            _curAtkTargetBuilding.ShowOutline(true);
        //        }
        //    }
        //}

        //private void FaceToNode(Node2D targetNode)
        //{
        //    float directionToTarget = targetNode.GlobalPosition.X - GlobalPosition.X;// 计算从自己指向敌人的向量
        //    if (directionToTarget < 0)
        //        SpriteRoot.Scale = new Vector2(-1, 1); // 目标在左
        //    else if (directionToTarget > 0)
        //        SpriteRoot.Scale = new Vector2(1, 1);  // 目标在右
        //}

        public void SwapItemInterInventory(string sourceInvGuid, int sourceIndex, string targetInvGuid, int targetIndex)
        {
            Inventory sourceInv = null;
            Inventory targetInv = null;

            if (sourceInvGuid == FastBarInventory.GuidStr)
                sourceInv = FastBarInventory;
            else if (sourceInvGuid == BagInventory.GuidStr)
                sourceInv = BagInventory;

            if (targetInvGuid == FastBarInventory.GuidStr)
                targetInv = FastBarInventory;
            else if (targetInvGuid == BagInventory.GuidStr)
                targetInv = BagInventory;

            if (sourceInv == null || targetInv == null)
                return;

            if (sourceInv.ItemInstanceList[sourceIndex] == null)
                return;

            if (targetInv.ItemInstanceList[targetIndex] == null)
            {
                targetInv.ItemInstanceList[targetIndex] = sourceInv.ItemInstanceList[sourceIndex];
                sourceInv.ItemInstanceList[sourceIndex] = null;
                sourceInv.SlotChanged?.Invoke(sourceIndex);
                targetInv.SlotChanged?.Invoke(targetIndex);
                return;
            }

            if (sourceInv.ItemInstanceList[sourceIndex].Type != targetInv.ItemInstanceList[targetIndex].Type)
            {
                ItemInstance temp = sourceInv.ItemInstanceList[sourceIndex];
                sourceInv.ItemInstanceList[sourceIndex] = targetInv.ItemInstanceList[targetIndex];
                targetInv.ItemInstanceList[targetIndex] = temp;
                sourceInv.SlotChanged?.Invoke(sourceIndex);
                targetInv.SlotChanged?.Invoke(targetIndex);
            }
            else
            {
                int maxCount = ItemDataManager.Instance.GetItemData(targetInv.ItemInstanceList[targetIndex].Type).MaxCount;
                int targetCount = targetInv.ItemInstanceList[targetIndex].Count;
                int sourceCount = sourceInv.ItemInstanceList[sourceIndex].Count;
                int canAddCount = maxCount - targetCount;
                if (canAddCount > 0)
                {
                    int addCount = sourceCount > canAddCount ? canAddCount : sourceCount;
                    targetInv.ItemInstanceList[targetIndex].Count += addCount;
                    sourceInv.ItemInstanceList[sourceIndex].Count -= addCount;
                    if (sourceInv.ItemInstanceList[sourceIndex].Count <= 0)
                        sourceInv.ItemInstanceList[sourceIndex] = null;
                    sourceInv.SlotChanged?.Invoke(sourceIndex);
                    targetInv.SlotChanged?.Invoke(targetIndex);
                }
            }
        }

        public int AddItemToInventory(ItemInstance itemInstance)
        {
            itemInstance.Count -= FastBarInventory.AddItemInstance(itemInstance);//优先添加到快捷栏
            if (itemInstance.Count != 0)//有剩余就添加到背包
            {
                itemInstance.Count -= BagInventory.AddItemInstance(itemInstance);
            }
            RefreshHandNode();
            return itemInstance.Count;
        }


        public int CurFastBarIndex;//玩家当前手持的物品, 攻击时要把这个连同攻击力一起传过去给受击者, 让受击者处理受多少伤害, 工具不对要大打折扣
        [Export] private Node2D _handNode;
        [Export] private Sprite2D _handSprite;

        private Node2D _curEquipmentNode = null;
        private void ChangeCurFastBarIndex(bool isNext)
        {
            if (isNext)
            {
                // 如果已经是最后一个格子，直接返回，不作任何操作
                if (CurFastBarIndex >= FastBarInventory.ItemInstanceList.Count - 1)
                    return;
                _fastBarInventoryView.SetSelected(CurFastBarIndex, false);
                CurFastBarIndex++;
            }
            else
            {
                if (CurFastBarIndex <= 0)
                    return;
                _fastBarInventoryView.SetSelected(CurFastBarIndex, false);
                CurFastBarIndex--;
            }
            _fastBarInventoryView.SetSelected(CurFastBarIndex, true);
            RefreshHandNode();
        }

        private void RefreshHandNode()
        {
            _handSprite.Texture = null;
            if (FastBarInventory.ItemInstanceList[CurFastBarIndex] == null)
                return;
            if (ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding == false)
            {
                _handSprite.Texture = GD.Load<Texture2D>(ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).IconPath);

            }
        }


        private Tween _shakeTween;
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

        public Vector2 GetWorldPosition()
        {
            return GlobalPosition;
        }
        public void TakeDamage(float damage, ItemType? itemType)
        {

        }

        public bool CanInteract()
        {
            return true;
        }

        public bool CanAtk()
        {
            return true;
        }

        public void ShowInteractTip(bool isShow)
        {
            return;
        }

        public void ShowAtkTip(bool isShow)
        {
            return;
        }

        public void ShowOutline(bool isShow)
        {
            return;
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


