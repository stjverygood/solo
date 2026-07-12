using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Global.Interfaces;
using System;
using System.Collections.Generic;
namespace Solo.Scripts.Entities.Zombies
{
    public enum ZombieState
    {
        Idle,
        Patrol,
        Chase,
        Atk,
        Death,
    }

    public partial class Zombie : CharacterBody2D, IEntity
    {
        [Export] private Label _debugLb = null!;
        public ZombieState _curState;

        private readonly Dictionary<Type, Component> _componentMap = new Dictionary<Type, Component>();

        public T GetComponent<T>()
        {
            if (_componentMap[typeof(T)] is not T component)
                return default;
            return component;
        }

        public void Init()
        {
            _componentMap[typeof(HpComponent)] = new HpComponent();
            _componentMap[typeof(AtkComponent)] = new AtkComponent();
            _componentMap[typeof(DefComponent)] = new DefComponent();
        }

        public override void _PhysicsProcess(double delta)
        {
            UpdateState((float)delta);
            _debugLb.Text = _curState.ToString();
        }

        private void ChangeState(ZombieState newState)
        {
            ExitState(_curState);
            _curState = newState;
            EnterState(newState);
        }

        private void EnterState(ZombieState state)
        {
            switch (state)
            {
                case ZombieState.Idle:
                    EnterIdle();
                    break;
                case ZombieState.Patrol:
                    EnterPatrol();
                    break;
                case ZombieState.Chase:
                    EnterChase();
                    break;
                case ZombieState.Atk:
                    EnterAtk();
                    break;
                case ZombieState.Death:
                    EnterDeath();
                    break;
            }
        }
        private void UpdateState(float delta)
        {
            switch (_curState)
            {
                case ZombieState.Idle:
                    UpdateIdle(delta);
                    break;
                case ZombieState.Patrol:
                    UpdatePatrol(delta);
                    break;
                case ZombieState.Chase:
                    UpdateChase(delta);
                    break;
                case ZombieState.Atk:
                    UpdateAtk(delta);
                    break;
                case ZombieState.Death:
                    UpdateDeath(delta);
                    break;
            }
        }
        private void ExitState(ZombieState state)
        {
            switch (state)
            {
                case ZombieState.Idle:
                    ExitIdle();
                    break;
                case ZombieState.Patrol:
                    ExitPatrol();
                    break;
                case ZombieState.Chase:
                    ExitChase();
                    break;
                case ZombieState.Atk:
                    ExitAtk();
                    break;
                case ZombieState.Death:
                    ExitDeath();
                    break;
            }
        }

        #region Idle
        private void EnterIdle()
        {

        }
        private void UpdateIdle(float delta)
        {

        }
        private void ExitIdle()
        {

        }
        #endregion

        #region Patrol
        private void EnterPatrol()
        {

        }
        private void UpdatePatrol(float delta)
        {

        }
        private void ExitPatrol()
        {

        }
        #endregion

        #region Chase
        private void EnterChase()
        {

        }
        private void UpdateChase(float delta)
        {

        }
        private void ExitChase()
        {

        }
        #endregion

        #region Atk
        private void EnterAtk()
        {

        }
        private void UpdateAtk(float delta)
        {

        }
        private void ExitAtk()
        {

        }
        #endregion

        #region Death
        private void EnterDeath()
        {

        }
        private void UpdateDeath(float delta)
        {

        }
        private void ExitDeath()
        {

        }


        #endregion
    }
}