using Input;
using UnityEngine;

namespace FSM
{
    public class PlayerFastFallState : PlayerInAirState
    {
        public PlayerFastFallState(PlayerStateMachine ctx, PlayerStateFactory factory) : base(ctx, factory)
        {
            IsRootState = false;
        }

        public override void EnterState()
        {
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
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

        public override void InitializeSubState()
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