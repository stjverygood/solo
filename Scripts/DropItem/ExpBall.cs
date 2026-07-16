using Godot;
using Solo.Scripts.Entities;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Global.Interfaces;

public partial class ExpBall : Area2D
{
    private IEntity? _curTargetEntity;
    private float _exp;


    public void Init(Vector2 worldPos, float exp)
    {
        GlobalPosition = worldPos;
        _exp = exp;
        BodyEntered += ExpBall_BodyEntered;
        BodyExited += ExpBall_BodyExited;

    }

    public override void _PhysicsProcess(double delta)
    {
        if (_curTargetEntity == null)
            return;

        if (GlobalPosition.DistanceSquaredTo(((Node2D)_curTargetEntity).GlobalPosition) < 5)
        {
            _curTargetEntity.GetComponent<ExpComponent>().Gain(_exp);
            QueueFree();
        }

        GlobalPosition += (((Node2D)_curTargetEntity).GlobalPosition - GlobalPosition).Normalized() * 50 * (float)delta;
    }

    private void ExpBall_BodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            _curTargetEntity = player;
        }
    }

    private void ExpBall_BodyExited(Node2D body)
    {
        if (body is Player player)
        {
            _curTargetEntity = null;
        }
    }
}
