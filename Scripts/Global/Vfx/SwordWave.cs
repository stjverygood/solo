using Godot;

public partial class SwordWave : Node2D
{
    [Export] private AnimatedSprite2D _animSprite;

    public void Play(Vector2 worldPositon, Vector2 atkDir)
    {
        GlobalPosition = worldPositon + atkDir * 10;
        Rotation = atkDir.Angle();

        if (GD.Randf() < 0.5f)
        {
            _animSprite.Play("Wave1");
        }
        else
        {
            _animSprite.Play("Wave2");
        }
    }
}
