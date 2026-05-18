using Solo.Global.Interfaces;
using Solo.Scripts.Global;
using Godot;
using System;
using System.Collections.Generic;

public enum PlayerState
{
    Idle,
    Move,
    Atk,
    Death,
}

public partial class Player : CharacterBody2D
{
	private Vector2 _curDir = Vector2.Right;
    private PlayerState _curState = PlayerState.Idle;
	public float moveSpeed = 200;
    [Export] public Sprite2D Sprite;
    [Export] public Area2D AtkArea;
    public int Atk = 10;

    public int ResCapacity = 1000;//资源存储上限
    private Tween _animTween; // 用于管理当前动画

    public override void _Ready()
    {
        AtkArea.Monitoring = false;
        AtkArea.BodyEntered += AtkArea_BodyEntered;
    }

   

    public override void _PhysicsProcess(double delta)
    {
        UpdateState((float)delta);
        //GD.Print($"curTileType : {GameManager.Instance.ChunkManager.GetTileType(GlobalPosition)}");
    }

    public void UpdateState(float delta)
    {
        switch(_curState)
        {
            case PlayerState.Idle:
                UpdateIdle(delta);
                break;
            case PlayerState.Move:
                UpdateMove(delta);
                break;
            case PlayerState.Atk:
                UpdateAtk(delta);
                break;
            case PlayerState.Death:
                UpdateDeath(delta);
                break;
        }
    }

    public void UpdateIdle(float delta)
    {
        if (Input.IsActionJustPressed("Atk"))
        {
            _curState = PlayerState.Atk;
            ChangeAnim();
            return;
        }

        Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
        if(input != Vector2.Zero)
        {
            _curState = PlayerState.Move;
            ChangeAnim();
            return;
        }
    }
    public void UpdateMove(float delta)
    {
        if(Input.IsActionJustPressed("Atk"))
        {
            _curState = PlayerState.Atk;
            ChangeAnim();
            return;
        }

        Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
        if(input == Vector2.Zero)
        {
            _curState = PlayerState.Idle;
            ChangeAnim();
            return;
        }
        _curDir = input;
        if (input.X < 0)
            Sprite.Scale = new Vector2(-1, 1);
        else if (input.X > 0)
            Sprite.Scale = new Vector2(1, 1);
        AtkArea.Rotation = input.Angle();
        if(GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
            Velocity = input * moveSpeed / 4;
        else
            Velocity = input * moveSpeed;
        MoveAndSlide();
    }
    public void UpdateAtk(float delta)
    {
        Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
        if (input != Vector2.Zero)
        {
            AtkArea.Rotation = input.Angle();
            if (GameManager.Instance.ChunkManager.GetTileType(GlobalPosition) == TileType.Water)
                Velocity = input * moveSpeed / 8;
            else
                Velocity = input * moveSpeed / 2;
            MoveAndSlide();
        }
    }
    public void UpdateDeath(float delta)
    {

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
        _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        switch (_curState)
        {
            case PlayerState.Idle:
                float currentDir = Sprite.Scale.X > 0 ? 1.0f : -1.0f;
                _animTween.SetLoops();
                _animTween.TweenProperty(Sprite, "scale", new Vector2(1.05f * currentDir, 0.95f), 0.5f);
                _animTween.TweenProperty(Sprite, "scale", new Vector2(1.0f * currentDir, 1.0f), 0.5f);
                break;

            case PlayerState.Move:
                // 走动效果：左右晃动或轻微拉伸
                _animTween.SetLoops();
                _animTween.TweenProperty(Sprite, "skew", 0.1f, 0.15f);
                _animTween.TweenProperty(Sprite, "skew", -0.1f, 0.15f);
                break;

            case PlayerState.Atk:
                float dashDistance = 50.0f; // 冲顶距离
                Vector2 startPos = Vector2.Zero;
                Vector2 attackDir = _curDir;
                Vector2 targetPos = attackDir * dashDistance;

                // 获取当前的镜像方向，用于保持缩放动画不反向
                float scaleX = Sprite.Scale.X;

                // --- 第一阶段：向前冲顶 ---
                //_animTween.TweenProperty(Sprite, "position", targetPos, 0.07f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
                _animTween.Parallel().TweenProperty(Sprite, "modulate", Colors.Black, 0.07f);
                // 缩放也要考虑当前的镜像状态
                _animTween.Parallel().TweenProperty(Sprite, "scale", new Vector2(1.3f, 2f), 0.07f);

                // --- 伤害触发 ---
                _animTween.TweenCallback(Callable.From(() =>
                {
                    //GD.Print($"朝方向 {attackDir} 触发伤害！");
                    AtkArea.Monitoring = true;
                }));
                _animTween.TweenInterval(0.1f);
                _animTween.TweenCallback(Callable.From(() =>
                {
                    ApplyDamage();
                }));


                // --- 第二阶段：收招归位 ---
                //_animTween.TweenProperty(Sprite, "position", startPos, 0.2f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
                _animTween.Parallel().TweenProperty(Sprite, "modulate", Colors.White, 0.2f);
                _animTween.Parallel().TweenProperty(Sprite, "scale", new Vector2(1.0f * Mathf.Sign(scaleX), 1.0f), 0.2f);

                _animTween.Finished += () => {
                    if (_curState == PlayerState.Atk)
                    {
                        _curState = PlayerState.Idle;
                        ChangeAnim();
                    }
                };
                break;

            case PlayerState.Death:
                // 死亡效果：变黑、旋转并消失
                _animTween.TweenProperty(Sprite, "modulate", new Color(0, 0, 0, 0), 0.8f);
                _animTween.Parallel().TweenProperty(Sprite, "rotation", Mathf.Pi, 0.8f);
                _animTween.Finished += () => QueueFree(); // 动画结束删除对象
                break;
        }
    }


    private List<IAttackable> _atkTargetList = new List<IAttackable>();
    private void AtkArea_BodyEntered(Node2D body)
    {
        if (body is IAttackable atkTarget)
        {
            _atkTargetList.Add(atkTarget);
        }
    }
    private void ApplyDamage()
    {
        foreach(var target in _atkTargetList)
        {
            target.TakeDamage(Atk);
        }
        AtkArea.Monitoring = false;
        _atkTargetList.Clear();
    }
}

