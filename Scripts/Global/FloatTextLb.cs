using Godot;

public partial class FloatTextLb : Label
{
    [Export] public float FloatDistance = 30.0f; // 向上漂浮的距离
    [Export] public double Duration = 1d;       // 动画持续时间


    [Export] public float MaxRandomAngle = 0.2f;// 允许的最大左右随机角度（弧度制
    [Export] public float PositionOffsetRange = 5.0f;// 初始位置的最大随机偏移像素

    public void Init(string text, Vector2 globalPos, Color? color = null)
    {
        Text = text;
        HorizontalAlignment = HorizontalAlignment.Center; // 文字水平居中
        VerticalAlignment = VerticalAlignment.Center;     // 文字垂直居中

        ResetSize();
        PivotOffset = Size / 2;

        if (color.HasValue)
        {
            Modulate = color.Value;
        }

        float randomX = (float)GD.RandRange(-PositionOffsetRange, PositionOffsetRange);
        float randomY = (float)GD.RandRange(-PositionOffsetRange, PositionOffsetRange);
        GlobalPosition = globalPos - (Size / 2) + new Vector2(randomX, randomY);
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