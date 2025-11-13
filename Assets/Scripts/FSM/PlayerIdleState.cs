using UnityEngine;
using Input;

namespace FSM
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerStateMachine ctx, PlayerStateFactory factory) : base(ctx, factory)
        {
            IsRootState = false;
        }

        public override void EnterState()
        {
            Debug.Log("Entering Idle State");
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void FixedUpdateState()
        {
        }

        public override void ExitState() { }

        public override void CheckSwitchStates()
        {
            if (InputManager.Movement.magnitude > 0)
            {
                SwitchState(Factory.Walk());
            }
        }

        public override void InitializeSubState() { }
        public override void OnTriggerEnter2D(Collider2D other) { }
        public override void OnTriggerStay2D(Collider2D other) { }
        public override void OnTriggerExit2D(Collider2D other) { }
    }
}