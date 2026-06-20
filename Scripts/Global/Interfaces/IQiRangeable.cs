using Godot;
namespace Solo.Scripts.Global.Interfaces
{
    public interface IQiRangeable
    {
        public float GetQiRange();
        public void ShowQiRange(bool isShow);
        public void ShowQiRangeForSeconds();
        public Vector2 GetWorldPos();
    }
}
