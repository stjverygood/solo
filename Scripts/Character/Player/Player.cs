using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.BuildingSystem;
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

            CurState = PlayerState.Idle;
            ChangeAnim();

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
            //Vector2 placePos = GlobalPosition + Vector2.Down * 20;
            //Vector2 snapPos = GameManager.Instance.ChunkManager.BuildingManager.SnapToCell(BuildingDataManager.Instance.GetItemData(TestStoneBuildingPreview.Type), placePos);
            //bool canPlace = GameManager.Instance.ChunkManager.BuildingManager.CanPlaced(BuildingDataManager.Instance.GetItemData(TestStoneBuildingPreview.Type), snapPos);
            //TestStoneBuildingPreview.SetCanPlace(canPlace);
            //GD.Print($"snapPos : {snapPos}");
            //GD.Print($"canPlace : {canPlace}");
            //TestStoneBuildingPreview.GlobalPosition = snapPos;
            //if (Input.IsActionJustPressed("Action") && canPlace)
            //{
            //    GameManager.Instance.ChunkManager.BuildingManager.Place(BuildingDataManager.Instance.GetItemData(TestStoneBuildingPreview.Type), snapPos);
            //    Building bd = TestStoneBuildingPs.Instantiate<Building>();
            //    bd.GlobalPosition = snapPos;
            //    GetTree().CurrentScene.AddChild(bd);
            //}


            UpdateState((float)delta);
            //GD.Print($"_curState : {_curState}");
            //GD.Print($"GlobalPosition : {GlobalPosition}");
        }

        public void UpdateState(float delta)
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
                //case PlayerState.Pickup:
                //    UpdatePickup(delta);
                //    break;
                case PlayerState.Death:
                    UpdateDeath(delta);
                    break;
                case PlayerState.BagUI:
                    UpdateBagUI(delta);
                    break;
            }
        }






        public void UpdateIdle(float delta)
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
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
            }

            if (Input.IsActionJustPressed("Bag"))
            {
                _selfView.Visible = true;
                _bagInventoryView.Visible = true;
                CurState = PlayerState.BagUI;
                return;
            }

            if (Input.IsActionJustPressed("Dash"))
            {
                _dashTimer = 0;
                CurState = PlayerState.Dash;
                ChangeAnim();
                return;
            }

            CheckAtkTarget();
            if (Input.IsActionJustPressed("Atk") && _curAtkTargetNode != null)
            {
                FaceToNode(_curAtkTargetNode);
                CurState = PlayerState.Atk;
                ChangeAnim();
                return;
            }
            //todo : enemy
            //优先级通过_curTargetBuilding来区分, _curTargetEnemy不为空就先打敌人, 不然就是打建筑
            //_curTargetBuilding = GetNearestNode();
            //if (_curTargetBuilding != null)
            //{
            //    FaceToNode(_curTargetBuilding);
            //    CurState = PlayerState.Atk;
            //    ChangeAnim();
            //    return;
            //}

            //_curDropItem = GetNearestDropItem();
            //if (_curDropItem != null)
            //{
            //    FaceToNode(_curDropItem);
            //    CurState = PlayerState.Pickup;
            //    ChangeAnim();
            //    return;
            //}
            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input != Vector2.Zero)
            {
                CurState = PlayerState.Walk;
                ChangeAnim();
                return;
            }
        }

        public void UpdateWalk(float delta)
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
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
            }

            if (Input.IsActionJustPressed("Bag"))
            {
                _selfView.Visible = true;
                _bagInventoryView.Visible = true;
                CurState = PlayerState.BagUI;
                return;
            }

            if (Input.IsActionJustPressed("Dash"))
            {
                _dashTimer = 0;
                CurState = PlayerState.Dash;
                ChangeAnim();
                return;
            }

            CheckAtkTarget();
            if (Input.IsActionJustPressed("Atk") && _curAtkTargetNode != null)
            {
                FaceToNode(_curAtkTargetNode);
                CurState = PlayerState.Atk;
                ChangeAnim();
                return;
            }

            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input == Vector2.Zero)
            {
                CurState = PlayerState.Idle;
                ChangeAnim();
                return;
            }
            _curDir = input;
            if (input.X < 0)
                SpriteRoot.Scale = new Vector2(-1, 1);
            else if (input.X > 0)
                SpriteRoot.Scale = new Vector2(1, 1);
            //TouchArea.Rotation = input.Angle();
            if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
                Velocity = input * moveSpeed / 4;
            else
                Velocity = input * moveSpeed;
            MoveAndSlide();
        }

        public void UpdateRun(float delta)
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
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
            }

            CheckAtkTarget();
            if (Input.IsActionJustPressed("Atk") && _curAtkTargetNode != null)
            {
                FaceToNode(_curAtkTargetNode);
                CurState = PlayerState.Atk;
                ChangeAnim();
                return;
            }

            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input == Vector2.Zero || Input.IsActionPressed("Dash") == false)
            {
                CurState = PlayerState.Idle;
                ChangeAnim();
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

        private float _dashTimer;
        private float _dashDuration = 0.2f;
        private float _dashSpeed = 200;
        public void UpdateDash(float delta)
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
            }
            if (Input.IsActionJustPressed("Next"))
            {
                ChangeCurFastBarIndex(true);
            }

            _dashTimer += delta;
            if (_dashTimer >= _dashDuration)
            {
                if (Input.IsActionPressed("Dash"))
                {
                    CurState = PlayerState.Run;
                    ChangeAnim();
                }
                else
                {
                    CurState = PlayerState.Idle;
                    ChangeAnim();
                }
            }
            Velocity = _curDir * _dashSpeed;
            MoveAndSlide();
        }

        public void UpdateAtk(float delta)
        {
            Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
            if (input != Vector2.Zero)
            {
                //TouchArea.Rotation = input.Angle();
                if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
                    Velocity = input * moveSpeed / 8;
                else
                    Velocity = input * moveSpeed / 2;
                MoveAndSlide();
            }
        }

        public void UpdateCapture(float delta)
        {
        }

        //public void UpdatePickup(float delta)
        //{
        //}

        public void UpdateDeath(float delta)
        {

        }

        public void UpdateBagUI(float delta)
        {
            if (Input.IsActionJustPressed("Bag") || Input.IsActionJustPressed("Back"))
            {
                _selfView.Visible = false;
                _bagInventoryView.Visible = false;
                CurState = PlayerState.Idle;
                return;
            }
        }

        private void Move()
        {

        }


        private void ChangeAnim()
        {

            if (_animTween != null && _animTween.IsRunning())//如果有正在运行的动画，先停止它
            {
                _animTween.Kill();
            }

            //复位
            BodyRoot.Scale = Vector2.One;
            BodyRoot.Skew = 0f;
            BodyRoot.Modulate = Colors.White;
            BodyRoot.Rotation = 0f;
            BodyRoot.Modulate = Colors.White;

            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            switch (CurState)
            {
                case PlayerState.Idle:
                    _animTween.SetLoops();
                    _animTween.TweenProperty(BodyRoot, "scale", new Vector2(1.2f, 0.8f), 0.5f);
                    _animTween.TweenProperty(BodyRoot, "scale", new Vector2(1.0f, 1.0f), 0.5f);
                    break;

                case PlayerState.Walk:
                case PlayerState.Dash:
                    _animTween.SetLoops();
                    _animTween.TweenProperty(BodyRoot, "skew", 0.1f, 0.3f);// 走动效果：左右晃动或轻微拉伸
                    _animTween.TweenProperty(BodyRoot, "skew", -0.1f, 0.3f);
                    break;

                case PlayerState.Run:
                    _animTween.SetLoops();
                    _animTween.TweenProperty(BodyRoot, "skew", 0.1f, 0.1f);// 走动效果：左右晃动或轻微拉伸
                    _animTween.TweenProperty(BodyRoot, "skew", -0.1f, 0.1f);
                    break;

                case PlayerState.Atk:
                    //_animTween.Parallel().TweenProperty(BodyRoot, "modulate", Colors.Red, 0.05f);
                    _animTween.TweenProperty(BodyRoot, "scale", new Vector2(0.8f, 0.8f), 0.05f); // 砍出去时身体拉伸
                    _animTween.TweenProperty(_handNode, "rotation", 1.3f, 0.05f); // 砍中目标的目标角度
                    _animTween.TweenCallback(Callable.From(() =>
                    {
                        if (_curAtkTargetNode != null)//todo : 如果是工具就扣耐久
                        {
                            if (_curAtkTargetNode is BuildingBase building)
                            {
                                if (FastBarInventory.ItemInstanceList[CurFastBarIndex] == null)//空手
                                {
                                    building.TakeDamage(null, Atk);
                                }
                                else
                                {
                                    building.TakeDamage(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type, Atk);
                                    if (ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).MaxDur != -1)
                                    {
                                        FastBarInventory.ItemInstanceList[CurFastBarIndex].CurDur--;
                                        FastBarInventory.ItemInstanceList[CurFastBarIndex].CurDur--;
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
                            //if is enemy
                        }
                    }));

                    _animTween.Parallel().TweenProperty(BodyRoot, "modulate", Colors.White, 0.1f);
                    _animTween.TweenProperty(BodyRoot, "scale", new Vector2(1f, 1f), 0.1f);
                    _animTween.TweenProperty(_handNode, "rotation", 0, 0.1f);
                    _animTween.Finished += () =>
                    {
                        CurState = PlayerState.Idle;
                        ChangeAnim();
                    };
                    break;

                case PlayerState.Death:
                    _animTween.TweenProperty(SpriteRoot, "modulate", new Color(0, 0, 0, 0), 0.8f);// 死亡效果：变黑、旋转并消失
                    _animTween.Parallel().TweenProperty(SpriteRoot, "rotation", Mathf.Pi, 0.8f);
                    _animTween.Finished += () => QueueFree(); // 动画结束删除对象
                    break;
            }
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
            if (body is BuildingBase building)//todo : enemy
            {
                _atkTargetNodeList.Add(building);

            }
        }
        private void AtkArea_BodyExited(Node2D body)
        {
            if (body is BuildingBase building)//todo : enemy
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
                if (IsInstanceValid(oldAtkTargetNode) && oldAtkTargetNode is BuildingBase oldBuilding)// 2. 关掉旧目标的文本（如果旧目标还存在的话）
                {
                    oldBuilding.ShowOutline(false);
                }
                if (IsInstanceValid(newAtkTargetNode) && newAtkTargetNode is BuildingBase newBuilding)// 3. 开启新目标的文本（如果新目标存在的话）
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
            _handSprite.Texture = GD.Load<Texture2D>(ItemDataManager.Instance.GetItemData(FastBarInventory.ItemInstanceList[CurFastBarIndex].Type).IconPath);
        }

    }
}


