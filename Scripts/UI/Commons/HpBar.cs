using Godot;

namespace Solo.Scripts.UI.Commons
{
    public partial class HpBar : Control
    {
        [Export] public TextureProgressBar _hpTpb = null!;
        [Export] public Label _hpLb = null!;
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
            _hpTpb.MaxValue = maxHp;
            _hpTpb.Value = curHp;
            _hpLb.Text = $"{curHp:f0}/{maxHp:f0}";
        }
    }
}
