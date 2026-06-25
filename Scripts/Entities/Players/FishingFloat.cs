using Godot;

public partial class FishingFloat : Node2D
{
    [Export] private Line2D _fishLine;
    [Export] private Sprite2D _floatSprite;

    [Export] private int _lineSegments = 8; // 鱼线细分的线段数量，越多越平滑
    [Export] private float _sagAmount = 30f; // 鱼线向下垂落的最大弧度（像素）

    private Vector2 _startPos;
    private Vector2 _endPos;
    private Tween _floatTween;

    public void Init(Vector2 startPos, Vector2 endPos)
    {
        // 保持父节点在起点位置
        GlobalPosition = startPos;
        _startPos = startPos;
        _endPos = endPos;

        // 确保清除旧线段
        _fishLine.ClearPoints();

        // 1. 播放抛竿入水动画
        PlayCastAnimation();
    }

    /// <summary>
    /// 抛竿动画：浮标从天而降，鱼线从绷直变为下垂
    /// </summary>
    private void PlayCastAnimation()
    {
        // 初始状态：浮标在空中（终点上方 40 像素），透明度为 0
        _floatSprite.GlobalPosition = _endPos + new Vector2(0, -40);
        _floatSprite.Modulate = new Color(1, 1, 1, 0);

        // 初始鱼线
        DrawBezierLine(_startPos, _floatSprite.GlobalPosition, 0);

        Tween castTween = CreateTween().SetParallel(true);

        // 浮标下落并淡入
        castTween.TweenProperty(_floatSprite, "global_position", _endPos, 0.5f)
            .SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        castTween.TweenProperty(_floatSprite, "modulate:a", 1.0f, 0.3f);

        // 动态更新鱼线
        castTween.TweenMethod(Callable.From<float>(currentSag =>
        {
            DrawBezierLine(_startPos, _floatSprite.GlobalPosition, currentSag);
        }), 0f, _sagAmount, 0.5f);

        // 抛竿动画结束后，开启水面浮动循环动画
        castTween.Chain().TweenCallback(Callable.From(StartFloatingAnimation));
    }

    /// <summary>
    /// 循环浮动动画：浮标上下微动，鱼线也随之微动
    /// </summary>
    private void StartFloatingAnimation()
    {
        _floatTween = CreateTween().SetLoops();

        Vector2 floatUp = _endPos + new Vector2(0, -3);
        Vector2 floatDown = _endPos + new Vector2(0, 3);

        // 阶段一：向上浮动
        _floatTween.TweenProperty(_floatSprite, "global_position", floatUp, 0.8f)
            .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        _floatTween.Parallel().TweenMethod(Callable.From<float>(currentSag =>
        {
            DrawBezierLine(_startPos, _floatSprite.GlobalPosition, currentSag);
        }), _sagAmount, _sagAmount - 2f, 0.8f);

        // 阶段二：向下沉动
        _floatTween.TweenProperty(_floatSprite, "global_position", floatDown, 0.8f)
            .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        _floatTween.Parallel().TweenMethod(Callable.From<float>(currentSag =>
        {
            DrawBezierLine(_startPos, _floatSprite.GlobalPosition, currentSag);
        }), _sagAmount - 2f, _sagAmount, 0.8f);
    }

    /// <summary>
    /// 核心算法：基于二次贝塞尔曲线绘制多点鱼线
    /// </summary>
    private void DrawBezierLine(Vector2 start, Vector2 end, float sag)
    {
        _fishLine.ClearPoints();

        // 计算控制点（全局坐标）
        Vector2 midPoint = (start + end) / 2f;
        Vector2 controlPoint = midPoint + new Vector2(0, sag);

        // 生成曲线上的多个点
        for (int i = 0; i <= _lineSegments; i++)
        {
            float t = (float)i / _lineSegments;
            // 二次贝塞尔曲线公式
            Vector2 globalPoint = (1f - t) * (1f - t) * start + 2f * (1f - t) * t * controlPoint + t * t * end;

            // 【核心修复】将全局坐标转换为 Line2D 的本地坐标
            Vector2 localPoint = _fishLine.ToLocal(globalPoint);
            _fishLine.AddPoint(localPoint);
        }
    }

    // 别忘了在节点销毁时杀掉可能在运行的循环 Tween，防止内存泄漏
    public override void _ExitTree()
    {
        if (_floatTween != null && _floatTween.IsValid())
        {
            _floatTween.Kill();
        }
    }
}