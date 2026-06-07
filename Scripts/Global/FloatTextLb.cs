using Godot;

public partial class FloatTextLb : Label
{
    [Export] public float FloatDistance = 30.0f; // 向上漂浮的距离
    [Export] public double Duration = 0.5d;       // 动画持续时间

    // 允许的最大左右随机角度（弧度制），约等于 35 度
    [Export] public float MaxRandomAngle = 0.6f;
    // 初始位置的最大随机偏移像素
    [Export] public float PositionOffsetRange = 10.0f;

    public void Init(string text, Vector2 globalPos, Color? color = null)
    {
        Text = text;

        // 1. 给初始位置增加一点点随机微调，防止多个数字完全重合
        float randomX = (float)GD.RandRange(-PositionOffsetRange, PositionOffsetRange);
        float randomY = (float)GD.RandRange(-PositionOffsetRange, PositionOffsetRange);
        GlobalPosition = globalPos + new Vector2(randomX, randomY);

        if (color.HasValue)
        {
            Modulate = color.Value;
        }

        StartFloatingAnimation();
    }

    private void StartFloatingAnimation()
    {
        Tween tween = CreateTween();
        tween.SetParallel(true);

        // 2. 基于正上方 (0, -1)，随机向左或向右偏转一定角度
        // GD.RandRange(-MaxRandomAngle, MaxRandomAngle) 决定了偏转方向和幅度
        float randomAngle = (float)GD.RandRange(-MaxRandomAngle, MaxRandomAngle);
        Vector2 randomDirection = Vector2.Up.Rotated(randomAngle);

        // 计算最终的终点
        Vector2 targetPosition = GlobalPosition + (randomDirection * FloatDistance);

        // 位置动画：带有一点点向左或向右的弧度抛出感
        tween.TweenProperty(this, "global_position", targetPosition, Duration)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.Out);

        // 透明度动画不变
        Color targetColor = Modulate;
        targetColor.A = 0;
        tween.TweenProperty(this, "modulate", targetColor, Duration)
             .SetTrans(Tween.TransitionType.Linear);

        // 动画结束后销毁
        tween.Chain().TweenCallback(Callable.From(QueueFree));
    }
}