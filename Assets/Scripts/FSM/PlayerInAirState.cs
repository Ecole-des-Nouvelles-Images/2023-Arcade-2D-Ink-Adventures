using UnityEngine;

namespace FSM
{
    public class PlayerInAirState : PlayerBaseState, IRootState
    {
        public PlayerInAirState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            InitiazeSubState();
        }

        public override void UpdateState()
        {
            throw new System.NotImplementedException();
        }

        public override void FixedUpdateState()
        {
            throw new System.NotImplementedException();
        }

        public override void ExitState()
        {
            throw new System.NotImplementedException();
        }

        public override void CheckSwitchStates()
        {
            throw new System.NotImplementedException();
        }

        public override void InitiazeSubState()
        {
            throw new System.NotImplementedException();
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
            throw new System.NotImplementedException();
        }

        public override void OnTriggerStay2D(Collider2D other)
        {
            throw new System.NotImplementedException();
        }

        public override void OnTriggerExit2D(Collider2D other)
        {
            throw new System.NotImplementedException();
        }

        public void HandleGravity()
        {
            throw new System.NotImplementedException();
        }
    }
}
