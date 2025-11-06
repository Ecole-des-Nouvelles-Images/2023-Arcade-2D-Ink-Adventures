using UnityEngine;

namespace FSM
{
    public class PlayerWalkState : PlayerBaseState
    {
        public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
        }

        public override void EnterState()
        {
        }

        public override void UpdateState()
        {
        }

        public override void FixedUpdateState()
        {
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
        }

        public override void InitiazeSubState()
        {
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
        }

        public override void OnTriggerStay2D(Collider2D other)
        {
        }

        public override void OnTriggerExit2D(Collider2D other)
        {
        }
    }
}
