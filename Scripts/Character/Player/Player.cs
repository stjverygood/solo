using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.InventorySystem;
using Solo.Scripts.System.ItemSystem;
using Solo.Scripts.System.SaveSystem;
using System.Collections.Generic;

namespace Solo.Scripts.Character.Player
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Dash,
        Atk,//近战攻击
        RangeAtk,//远程攻击
        Build,//建造状态
        Death,
        BagUI,
    }

    public partial class Player : CharacterBody2D
    {
        private Vector2 _curDir = Vector2.Right;
        public PlayerState CurState;
        public float moveSpeed = 50;
        [Export] public Node2D SpriteRoot;//附带上身体之外的交互点, 比如后面拍建筑的定位点, 用于控制功能交互的
        [Export] public Node2D BodyRoot;//仅仅是身体的根节点, 用于控制动画
        [Export] public Area2D InteractArea;
        [Export] public Area2D AtkArea;
        [Export] public Area2D RangeAtkArea;
        [Export] public BuildingPreview TestStoneBuildingPreview;
        [Export] public PackedScene TestStoneBuildingPs;
        public float Atk = 10;

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


            InteractArea.BodyEntered += InteractArea_BodyEntered;
            InteractArea.BodyExited += InteractArea_BodyExited;
            AtkArea.BodyEntered += AtkArea_BodyEntered;
            AtkArea.BodyExited += AtkArea_BodyExited;
            RangeAtkArea.BodyEntered += RangeAtkArea_BodyEntered;
            RangeAtkArea.BodyExited += RangeAtkArea_BodyExited;


            if (FastBarInventory.ItemInstanceList[CurFastBarIndex] != null && ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding)
            {
                ChangeState(PlayerState.Build);
            }
            else
            {
                ChangeState(PlayerState.Idle);
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
            //GD.Print($"CurState : {CurState}");
            GD.Print($"GlobalPosition : {GlobalPosition}");
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
                case PlayerState.Atk:
                    EnterAtk();
                    break;
                case PlayerState.RangeAtk:
                    EnterRangeAtk();
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
                case PlayerState.Atk:
                    UpdateAtk(delta);
                    break;
                case PlayerState.RangeAtk:
                    UpdateRangeAtk(delta);
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

        #region idle
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

            CheckAtkTarget();
            if (Input.IsActionJustPressed("Atk") && _curAtkTargetNode != null)
            {
                ChangeState(PlayerState.Atk);
                return;
            }

            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input != Vector2.Zero)
            {
                ChangeState(PlayerState.Walk);
                return;
            }

            CheckInteractTarget();
            if (Input.IsActionJustPressed("Interact") && _curInteractTargetNode != null)
            {
                if (_curInteractTargetNode is DropItem dropItem)
                    dropItem.Pickup();
            }
        }
        #endregion

        #region walk
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

            CheckAtkTarget();
            if (Input.IsActionJustPressed("Atk") && _curAtkTargetNode != null)
            {
                ChangeState(PlayerState.Atk);
                return;
            }
            CheckInteractTarget();
            if (Input.IsActionJustPressed("Interact") && _curInteractTargetNode != null)
            {
                if (_curInteractTargetNode is DropItem dropItem)
                    dropItem.Pickup();
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

        #region run
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

            CheckInteractTarget();
            if (Input.IsActionJustPressed("Interact") && _curInteractTargetNode != null)
            {
                if (_curInteractTargetNode is DropItem dropItem)
                    dropItem.Pickup();
            }
            CheckAtkTarget();
            if (Input.IsActionJustPressed("Atk") && _curAtkTargetNode != null)
            {
                ChangeState(PlayerState.Atk);
                return;
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

        #region dash
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
            CheckInteractTarget();
            if (Input.IsActionJustPressed("Interact") && _curInteractTargetNode != null)
            {
                if (_curInteractTargetNode is DropItem dropItem)
                    dropItem.Pickup();
            }

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

        #region atk
        private void EnterAtk()
        {
            ResetAnim();
            FaceToNode(_curAtkTargetNode);
            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            _animTween.Parallel().TweenProperty(BodyRoot, "scale", new Vector2(0.8f, 0.8f), 0.05f);//出手动画
            _animTween.TweenProperty(_handNode, "rotation", 1.3f, 0.05f);
            _animTween.TweenCallback(Callable.From(() =>
            {
                //if (_curAtkTargetNode == null)
                //    return;
                if (_curAtkTargetNode is Building building)
                {
                    if (FastBarInventory.ItemInstanceList[CurFastBarIndex] == null)//空手
                    {
                        building.TakeDamage(null, Atk);
                    }
                    else//工具, 要扣耐久
                    {
                        building.TakeDamage(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type, Atk);
                        if (ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).MaxDur != -1)
                        {
                            FastBarInventory.ItemInstanceList[CurFastBarIndex].CurDur--;
                            if (FastBarInventory.ItemInstanceList[CurFastBarIndex].CurDur <= 0)
                            {
                                FastBarInventory.RemoveItem(CurFastBarIndex);
                                RefreshHandNode();
                            }
                            _fastBarInventoryView.RefreshSlot(CurFastBarIndex);
                        }
                    }
                }
                //else if is enemy
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

        #region rangeAtk
        private void EnterRangeAtk()
        {
            ResetAnim();
        }
        private void UpdateRangeAtk(float delta)
        {
        }
        #endregion

        #region build
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
            _curBuildingPreview.Init(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type);
        }
        private void UpdateBuild(float delta)
        {
            if (Input.IsActionJustPressed("Pre"))
            {
                ChangeCurFastBarIndex(false);
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] == null || ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding == false)
                {
                    ChangeState(PlayerState.Idle);
                    return;
                }
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] == null || ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).isBuilding == false)
                {
                    ChangeState(PlayerState.Idle);
                    return;
                }
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

            if (Input.IsActionJustPressed("Atk"))
            {
                bool isBuild = _curBuildingPreview.Build();                if (isBuild)
                {
                    int remainCount = FastBarInventory.RemoveItemByIndex(CurFastBarIndex, 1);

                    if (remainCount == 0)//count = 0, 空手, 并且切到Idle
                    {
                        ChangeState(PlayerState.Idle);
                        return;
                    }
                }

            }

            if (Input.IsActionJustPressed("Interact"))
            {
                _curBuildingPreview.ChangeDir();
            }

            _curBuildingPreview.Update(GlobalPosition);
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

        private List<Node2D> _interactTargetNodeList = new List<Node2D>();
        private Node2D _curInteractTargetNode;
        private void InteractArea_BodyEntered(Node2D body)
        {
            if (body is DropItem dropItem)
            {
                _interactTargetNodeList.Add(dropItem);
            }
        }
        private void InteractArea_BodyExited(Node2D body)
        {
            if (body is DropItem dropItem)
            {
                _interactTargetNodeList.Remove(dropItem);
            }
        }
        private void CheckInteractTarget()
        {
            Node2D oldInteractTargetNode = _curInteractTargetNode;
            Node2D newInteractTargetNode = GlobalHelper.GetNearestNode(GlobalPosition, _interactTargetNodeList);
            if (oldInteractTargetNode != newInteractTargetNode)// 1. 只有当最近的物体“发生改变”时，才处理开关逻辑
            {
                if (IsInstanceValid(oldInteractTargetNode) && oldInteractTargetNode is DropItem oldDropItem)// 2. 关掉旧目标的文本（如果旧目标还存在的话）
                {
                    oldDropItem.ShowText(false);
                }
                if (IsInstanceValid(newInteractTargetNode) && newInteractTargetNode is DropItem newDropItem)// 3. 开启新目标的文本（如果新目标存在的话）
                {
                    newDropItem.ShowText(true);
                }
                _curInteractTargetNode = newInteractTargetNode;// 4. 交接变量，完成记忆更新
            }
        }

        private List<Node2D> _atkTargetNodeList = new List<Node2D>();
        private Node2D _curAtkTargetNode;
        private void AtkArea_BodyEntered(Node2D body)
        {
            if (body is Building building)//todo : enemy
            {
                _atkTargetNodeList.Add(building);

            }
        }
        private void AtkArea_BodyExited(Node2D body)
        {
            if (body is Building building)//todo : enemy
            {
                _atkTargetNodeList.Remove(building);
            }
        }
        private void CheckAtkTarget()
        {
            Node2D oldAtkTargetNode = _curAtkTargetNode;
            Node2D newAtkTargetNode = GlobalHelper.GetNearestNode(GlobalPosition, _atkTargetNodeList);
            if (oldAtkTargetNode != newAtkTargetNode)// 1. 只有当最近的物体“发生改变”时，才处理开关逻辑
            {
                if (IsInstanceValid(oldAtkTargetNode) && oldAtkTargetNode is Building oldBuilding)// 2. 关掉旧目标的文本（如果旧目标还存在的话）
                {
                    oldBuilding.ShowOutline(false);
                }
                if (IsInstanceValid(newAtkTargetNode) && newAtkTargetNode is Building newBuilding)// 3. 开启新目标的文本（如果新目标存在的话）
                {
                    newBuilding.ShowOutline(true);
                }
                _curAtkTargetNode = newAtkTargetNode;// 4. 交接变量，完成记忆更新
            }
        }

        private void RangeAtkArea_BodyExited(Node2D body)
        {
            //throw new global::System.NotImplementedException();
        }

        private void RangeAtkArea_BodyEntered(Node2D body)
        {
            //throw new global::System.NotImplementedException();
        }

        private void FaceToNode(Node2D targetNode)
        {
            float directionToTarget = targetNode.GlobalPosition.X - GlobalPosition.X;// 计算从自己指向敌人的向量
            if (directionToTarget < 0)
                SpriteRoot.Scale = new Vector2(-1, 1); // 目标在左
            else if (directionToTarget > 0)
                SpriteRoot.Scale = new Vector2(1, 1);  // 目标在右
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

    }
}


