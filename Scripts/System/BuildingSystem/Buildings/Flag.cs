using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.System.BuildingSystem;

public partial class Flag : Building, IQiRangeable
{
    [Export] private ColorRect _QiRangeCr;
    private float _qiRange = 50f;//灵气范围

    public override void Init(BuildingType type, Vector2 snapPos)
    {
        base.Init(type, snapPos);
        ShowQiRange(false);
        GameManager.Instance.IQiRangeableList.Add(this);
    }

    public float GetQiRange()
    {
        return _qiRange;
    }
    public Vector2 GetWorldPos()
    {
        return GlobalPosition;
    }
    public void ShowQiRange(bool isShow)
    {
        _QiRangeCr.Visible = isShow;
    }
    private Tween _qiTween;
    public void ShowQiRangeForSeconds()
    {
        ShowQiRange(true);
        if (_qiTween != null && _qiTween.IsValid())
        {
            _qiTween.Kill();
        }
        _qiTween = CreateTween();
        _qiTween.TweenCallback(Callable.From(() => ShowQiRange(false))).SetDelay(3.0f);
    }
    public override void Interact()
    {
        base.Interact();
        foreach (IQiRangeable qiRangeable in GameManager.Instance.IQiRangeableList)
        {
            qiRangeable.ShowQiRangeForSeconds();
        }
    }

    protected override void Die()
    {
        base.Die();
        GameManager.Instance.IQiRangeableList.Remove(this);
    }


}
