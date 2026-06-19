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
        private Vector2 _curDir = Vector2.Right;
        public PlayerState CurState;
        [Export] public Node2D SpriteRoot;//附带上身体之外的交互点, 比如后面拍建筑的定位点, 用于控制功能交互的
        [Export] public Node2D BodyRoot;//仅仅是身体的根节点, 用于控制动画
        [Export] private Sprite2D _sprite;
        [Export] private Camera2D _camera;


        [Export] public PackedScene DropItemPs;

        private ShaderMaterial _shaderMaterial;

        //人物属性
        public Vector2 StartPoint = new Vector2(0, 0);//出生点
        public float moveSpeed = 500;
        public float Atk = 10;
        private float _maxHp = 100;
        private float _maxHg = 100;
        private float _curHp;
        private float _curHg;


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
        [Export] private ProgressBar _hpPb;
        [Export] private Label _hpLb;
        [Export] private ProgressBar _hgPb;
        [Export] private Label _hgLb;
        [Export] private Label _debugLb;
        [Export] private DeathView _deathView;


        public override void _Ready()
        {
            GD.Print("Player Ready~~~");
            GameManager.Instance.Player = this;
            PlayerSaveData playerSaveData = SaveManager.Instance.CurSaveData.PlayerSaveData; //从存档里加载属性
            StartPoint = new Vector2(playerSaveData.StartPosX, playerSaveData.StartPosY);
            GlobalPosition = new Vector2(playerSaveData.PosX, playerSaveData.PosY);
            _maxHp = playerSaveData.MaxHp;
            SetCurHp(playerSaveData.CurHp);
            _maxHg = playerSaveData.MaxHg;
            SetCurHg(playerSaveData.CurHg);

            BagInventory.GuidStr = SaveManager.Instance.CurSaveData.BagInventoryGuidStr;
            BagInventory.ItemInstanceList = SaveManager.Instance.CurSaveData.BagInventoryList;
            FastBarInventory.GuidStr = SaveManager.Instance.CurSaveData.FastBarInventoryGuidStr;
            FastBarInventory.ItemInstanceList = SaveManager.Instance.CurSaveData.FastBarInventoryList;
            CurFastBarIndex = SaveManager.Instance.CurSaveData.FastBarIndex;

            if (_sprite.Material is ShaderMaterial shaderMat)
            {
                _shaderMaterial = (ShaderMaterial)shaderMat.Duplicate();// 关键：复制一份材质，确保每个实例的材质相互独立
                _sprite.Material = _shaderMaterial;// 记得把复制后的独立材质重新赋给当前的 Sprite2D
            }
            ShowOutline(false);

            _curAtkRange = _meleeAtkRange;//todo : 根据itemdata的israngeitem来决定攻击范围
            _curAtkRangeSq = _curAtkRange * _curAtkRange;
            _interactRangeSq = _interactRange * _interactRange;

            _curTargetRangeSq = _curTargetRange * _curTargetRange;
            _deathView.Visible = false;

            if (_curHp == 0)//若血量是0, 进入重生逻辑
            {
                Restart();
            }
            ChangeState(PlayerState.Idle);

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
            _debugLb.Text = CurState.ToString();
            if (Input.IsActionJustPressed("Test"))
            {
                TakeDamage(10, null);
            }
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
            _animTween.TweenProperty(BodyRoot, "scale", new Vector2(1.1f, 0.9f), 0.5f);
            _animTween.TweenProperty(BodyRoot, "scale", new Vector2(1.0f, 1.0f), 0.5f);
        }
        private void UpdateIdle(float delta)
        {
            if (_curHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }

            if (Input.IsActionJustPressed("Pre"))
            {
                ChangeCurFastBarIndex(false);
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
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
            if (_curHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }

            if (Input.IsActionJustPressed("Pre"))
            {
                ChangeCurFastBarIndex(false);
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
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
            if (_curHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }

            if (Input.IsActionJustPressed("Pre"))
            {
                ChangeCurFastBarIndex(false);
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
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
            if (_curHp <= 0)
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

            Velocity = _curDir * _dashSpeed;
            MoveAndSlide();
        }
        #endregion

        #region Atk
        private void EnterAtk()
        {
            ResetAnim();

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
                TriggerScreenShake(1);//震屏
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
                return;
            };
        }
        private void UpdateAtk(float delta)
        {
            if (_curHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }

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
            if (_curHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }
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
            if (_curHp <= 0)
            {
                ChangeState(PlayerState.Death);
                return;
            }

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
                bool isBuild = _curBuildingPreview.Build(mousePos);                if (isBuild)
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
                _curBuildingPreview.QueueFree();
                return;
            }
        }
        #endregion

        #region Death
        private void EnterDeath()
        {
            SetCurHp(0);
            CollisionLayer = 0;
            CollisionMask = 0;
            //todo :
            //1. 播放死亡动画
            ResetAnim();
            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            _animTween.TweenProperty(SpriteRoot, "scale", new Vector2(0.01f, 0.01f), 1f);
            _animTween.Finished += () =>
            {
                //2. 爆装备
                // 遍历快捷栏和背包, 把每个物品重新包装回dropItem, 在半径内随机生成, 然后调init
                for (int i = 0; i < FastBarInventory.ItemInstanceList.Count; i++)
                {
                    if (FastBarInventory.ItemInstanceList[i] == null)
                        continue;

                    ItemInstance itemInstance = FastBarInventory.ItemInstanceList[i];
                    DropItem dropItem = DropItemPs.Instantiate<DropItem>();
                    float randomAngle = (float)GD.RandRange(0, Mathf.Tau);
                    float randomRadius = Mathf.Sqrt((float)GD.Randf()) * 30f;
                    Vector2 randomOffset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomRadius;
                    Vector2 randomPosition = GlobalPosition + randomOffset;
                    GetTree().CurrentScene.AddChild(dropItem);
                    dropItem.Init(itemInstance, randomPosition);
                    dropItem.ApplyForce();
                }
                for (int i = 0; i < BagInventory.ItemInstanceList.Count; i++)
                {
                    if (BagInventory.ItemInstanceList[i] == null)
                        continue;

                    ItemInstance itemInstance = BagInventory.ItemInstanceList[i];
                    DropItem dropItem = DropItemPs.Instantiate<DropItem>();
                    float randomAngle = (float)GD.RandRange(0, Mathf.Tau);
                    float randomRadius = Mathf.Sqrt((float)GD.Randf()) * 30f;
                    Vector2 randomOffset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomRadius;
                    Vector2 randomPosition = GlobalPosition + randomOffset;
                    GetTree().CurrentScene.AddChild(dropItem);
                    dropItem.Init(itemInstance, randomPosition);
                    dropItem.ApplyForce();
                }

                //3. 等待1s
                GetTree().CreateTimer(1).Timeout += () =>
                {
                    //弹出菜单 : 退出到主菜单 or 复活
                    _deathView.Visible = true;
                };
            };
        }
        private void UpdateDeath(float delta)
        {
            if (_curHp > 0)
            {

                ChangeState(PlayerState.Idle);
                return;
            }
        }
        #endregion

        #region BagUI
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
                ChangeState(PlayerState.Idle);
                return;
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

        private void SetCurHp(float curHp)
        {
            _curHp = curHp;
            _hpPb.MaxValue = _maxHp;
            _hpPb.Value = _curHp;
            _hpLb.Text = $"{_curHp}/{_maxHp}";
        }
        private void SetCurHg(float curHg)
        {
            _curHg = curHg;
            _hgPb.MaxValue = _maxHg;
            _hgPb.Value = _curHg;
            _hgLb.Text = $"{_curHg}/{_maxHp}";
        }

        public void Restart()
        {
            GlobalPosition = StartPoint;
            SetCurHp(_maxHp);
            SetCurHg(_maxHg);
            CollisionLayer = 1;
            CollisionMask = 1;
        }

        public PlayerSaveData GetSaveData()
        {
            return new PlayerSaveData()
            {
                StartPosX = StartPoint.X,
                StartPosY = StartPoint.Y,
                PosX = GlobalPosition.X,
                PosY = GlobalPosition.Y,
                MaxHp = _maxHp,
                CurHp = _curHp,
                MaxHg = _maxHg,
                CurHg = _curHg,
            };
        }

        public Vector2 GetWorldPosition()
        {
            return GlobalPosition;
        }
        public void TakeDamage(float damage, ItemType? itemType)
        {
            Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            animTween.TweenProperty(SpriteRoot, "scale", new Vector2(0.8f, 0.8f), 0.1f);
            animTween.Parallel().TweenProperty(_sprite.Material, "shader_parameter/flash_modifier", 1.0f, 0.1f);
            animTween.TweenProperty(SpriteRoot, "scale", new Vector2(1.2f, 1.2f), 0.1f);
            animTween.Parallel().TweenProperty(_sprite.Material, "shader_parameter/flash_modifier", 0.0f, 0.1f);
            animTween.TweenProperty(SpriteRoot, "scale", new Vector2(1f, 1f), 0.1f);
            FloatTextLb floatTextLb = GameManager.Instance.FloatTextLbPs.Instantiate<FloatTextLb>();
            GetTree().CurrentScene.AddChild(floatTextLb);
            floatTextLb.Init($"-{damage}", GlobalPosition, new Color(162 / 256f, 38 / 256f, 51 / 256f));//162, 38, 51
            SetCurHp(_curHp - damage);
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

        public TargetType GetTargetType()
        {
            return TargetType.Player;
        }
    }
}


