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
        Atk,//第一优先级
        //Capture,//第二优先级
        Pickup,//第三优先级
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
        [Export] public Area2D TouchArea;
        [Export] public BuildingPreview TestStoneBuildingPreview;
        [Export] public PackedScene TestStoneBuildingPs;
        public int Atk = 10;

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

            TouchArea.BodyEntered += TouchArea_BodyEntered;
            TouchArea.BodyExited += TouchArea_BodyExited;
            TouchArea.AreaEntered += TouchArea_AreaEntered;
            TouchArea.AreaExited += TouchArea_AreaExited;
            CurState = PlayerState.Idle;
            ChangeAnim();

            _selfView.Visible = false;
            _bagInventoryView.Init(BagInventory);
            _fastBarInventoryView.Init(FastBarInventory);
            _bagInventoryView.Visible = false;
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
                case PlayerState.Pickup:
                    UpdatePickup(delta);
                    break;
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
            //if (Input.IsActionJustPressed("Back"))
            //{
            //    //通知暂停
            //    GameManager.Instance.ChangeState(GameState.Pause);
            //}

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

            if (Input.IsActionJustPressed("Action"))
            {
                //todo : enemy
                //优先级通过_curTargetBuilding来区分, _curTargetEnemy不为空就先打敌人, 不然就是打建筑
                _curTargetBuilding = GetNearestBuilding();
                if (_curTargetBuilding != null)
                {
                    FaceToNode(_curTargetBuilding);
                    CurState = PlayerState.Atk;
                    ChangeAnim();
                    return;
                }

                _curDropItem = GetNearestDropItem();
                if (_curDropItem != null)
                {
                    FaceToNode(_curDropItem);
                    CurState = PlayerState.Pickup;
                    ChangeAnim();
                    return;
                }
            }

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

            if (Input.IsActionJustPressed("Action"))
            {
                _curTargetBuilding = GetNearestBuilding();
                if (_curTargetBuilding != null)
                {
                    FaceToNode(_curTargetBuilding);
                    CurState = PlayerState.Atk;
                    ChangeAnim();
                    return;
                }

                _curDropItem = GetNearestDropItem();
                if (_curDropItem != null)
                {
                    FaceToNode(_curDropItem);
                    CurState = PlayerState.Pickup;
                    ChangeAnim();
                    return;
                }
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
            if (Input.IsActionJustPressed("Action"))
            {
                _curTargetBuilding = GetNearestBuilding();
                if (_curTargetBuilding != null)
                {
                    FaceToNode(_curTargetBuilding);
                    CurState = PlayerState.Atk;
                    ChangeAnim();
                    return;
                }

                _curDropItem = GetNearestDropItem();
                if (_curDropItem != null)
                {
                    FaceToNode(_curDropItem);
                    CurState = PlayerState.Pickup;
                    ChangeAnim();
                    return;
                }
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

        public void UpdatePickup(float delta)
        {
        }

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
                    _animTween.Parallel().TweenProperty(BodyRoot, "modulate", Colors.Black, 0.05f);//变黑
                    _animTween.TweenProperty(BodyRoot, "skew", 1f, 0.05);
                    _animTween.TweenCallback(Callable.From(() =>
                    {
                        if (_curTargetBuilding != null)
                        {
                            _curTargetBuilding.TakeDamage(Atk);
                        }
                    }));

                    _animTween.Parallel().TweenProperty(BodyRoot, "modulate", Colors.White, 0.1f);
                    _animTween.TweenProperty(BodyRoot, "skew", 0f, 0.1f);
                    _animTween.Finished += () =>
                    {
                        CurState = PlayerState.Idle;
                        ChangeAnim();
                    };
                    break;

                case PlayerState.Pickup:
                    _animTween.Parallel().TweenProperty(BodyRoot, "modulate", Colors.Black, 0.05f);//变黑
                    _animTween.TweenProperty(BodyRoot, "skew", 1f, 0.05);
                    _animTween.TweenCallback(Callable.From(() =>
                    {
                        if (_curDropItem != null)
                        {
                            _curDropItem.AddToPlayerInventory(FastBarInventory, BagInventory);
                            //var fastBarLogs = _fastBarInventory.ItemInstanceList.Select(item => item == null ? "null" : $"{item.Data.Type}:{item.Count}");
                            //GD.Print("--- 快捷栏 --- " + string.Join(" | ", fastBarLogs));
                            //var bagLogs = _bagInventory.ItemInstanceList.Select(item => item == null ? "null" : $"{item.Data.Type}:{item.Count}");
                            //GD.Print("--- 背  包 --- " + string.Join(" | ", bagLogs));
                        }
                    }));

                    _animTween.Parallel().TweenProperty(BodyRoot, "modulate", Colors.White, 0.1f);
                    _animTween.TweenProperty(BodyRoot, "skew", 0f, 0.1f);
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

        //后期加入enemylist

        private List<BuildingBase> _buildingList = new List<BuildingBase>();
        private BuildingBase _curTargetBuilding;
        private void TouchArea_BodyEntered(Node2D body)
        {
            if (body is BuildingBase building)
            {
                _buildingList.Add(building);
            }
        }
        private void TouchArea_BodyExited(Node2D body)
        {
            if (body is BuildingBase building)
            {
                _buildingList.Remove(building);
            }
        }
        private BuildingBase GetNearestBuilding()
        {
            float minDistSq = float.MaxValue; // 先设为一个极大值
            BuildingBase targetBuilding = null;
            foreach (BuildingBase curBuilding in _buildingList)
            {
                if (curBuilding == null)// 安全检查：防止列表里有被销毁的无效对象
                    continue;
                float curDistSq = GlobalPosition.DistanceSquaredTo(curBuilding.GlobalPosition);
                if (curDistSq < minDistSq)
                {
                    minDistSq = curDistSq;
                    targetBuilding = curBuilding;
                }
            }
            return targetBuilding;
        }

        List<DropItem> _dropItemList = new List<DropItem>();
        DropItem _curDropItem;
        private void TouchArea_AreaEntered(Area2D area)
        {
            if (area.GetParent() is DropItem dropItem)
            {
                _dropItemList.Add(dropItem);
                dropItem.ShowText(true);
            }
        }
        private void TouchArea_AreaExited(Area2D area)
        {
            if (area.GetParent() is DropItem dropItem)
            {
                _dropItemList.Remove(dropItem);
                dropItem.ShowText(false);
            }
        }
        private DropItem GetNearestDropItem()//返回掉落物
        {
            float minDistSq = float.MaxValue; // 先设为一个极大值
            DropItem nearestDropItem = null;
            foreach (DropItem curDropItem in _dropItemList)
            {
                if (curDropItem == null)// 安全检查：防止列表里有被销毁的无效对象
                    continue;
                float curDistSq = GlobalPosition.DistanceSquaredTo(curDropItem.GlobalPosition);
                if (curDistSq < minDistSq)
                {
                    minDistSq = curDistSq;
                    nearestDropItem = curDropItem;
                }
            }
            return nearestDropItem;
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
                int maxCount = ItemManager.Instance.GetItemData(targetInv.ItemInstanceList[targetIndex].Type).MaxCount;
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


    }
}


