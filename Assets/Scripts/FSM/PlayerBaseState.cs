using System;
using UnityEngine;

namespace FSM
{
    public abstract class PlayerBaseState : MonoBehaviour
    {
        private bool _isRootState = false;
        private PlayerStateMachine _ctx;
        private PlayerStateFactory _factory;
        private PlayerBaseState _currentSuperState;
        private PlayerBaseState _currentSubState;

        public bool IsRootState { set => _isRootState = value; }
        protected PlayerStateMachine Ctx => _ctx;
        protected PlayerStateFactory Factory => _factory;

        public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        {
            _ctx = currentContext;
            _factory = playerStateFactory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void FixedUpdateState();
        public abstract void ExitState();
        public abstract void CheckSwitchStates();
        public abstract void InitiazeSubState();

        private void UpdateStates()
        {
            UpdateState();
            if (_currentSubState != null)
            {
                _currentSubState.UpdateStates();
            }
        }

        private void FixedUpdateStates()
        {
            FixedUpdateState();
            if (_currentSubState != null)
            {
                _currentSubState.FixedUpdateStates();
            }
        }

        protected void SwitchState(PlayerBaseState newState)
        {
            ExitState();
            newState.EnterState();
            if (_isRootState)
            {
                _ctx.CurrentState = newState;
            }
        }

        private void SetSuperState(PlayerBaseState newSuperState)
        {
            _currentSuperState = newSuperState;
        }

        protected void SetSubState(PlayerBaseState newSubState)
        {
            _currentSubState = newSubState;
            newSubState.SetSuperState(this);
        }

        public abstract void OnTriggerEnter2D(Collider2D other);
        public abstract void OnTriggerStay2D(Collider2D other);
        public abstract void OnTriggerExit2D(Collider2D other);

    }
}
