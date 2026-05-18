using Solo.Global.Interfaces;
using Godot;
using System;

namespace Solo.Scripts.System.ResourceSystem
{
    public partial class ResourceBase : Node2D, IAttackable
    {

        public int MaxHp = 30;
        private int _curHp;

        public override void _Ready()
        {
            _curHp = MaxHp;

        }

        public override void _Process(double delta)
        {
        }

        public void TakeDamage(int damage)
        {
            GD.Print("get damage : " + damage);
            _curHp -= damage;
            if (_curHp <= 0)
            {
                _curHp = 0;
                QueueFree();
            }
        }
    }
}


