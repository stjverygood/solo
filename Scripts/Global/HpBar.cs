using Godot;
namespace Solo.Scripts.Global
{
    public partial class HpBar : ProgressBar
    {
        [Export] public Label _hpLb;
        public void Init()
        {

        }

        public void Refresh(float curHp, float maxHp)
        {
            if (curHp == maxHp)
            {
                Visible = false;
                return;
            }
            Visible = true;
            MaxValue = maxHp;
            Value = curHp;
            _hpLb.Text = $"{curHp:f0}/{maxHp:f0}";
        }
    }
}
