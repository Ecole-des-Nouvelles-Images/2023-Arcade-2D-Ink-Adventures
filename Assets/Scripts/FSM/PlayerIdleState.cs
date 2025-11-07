using UnityEngine;
using Input;

namespace FSM
{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerStateMachine ctx, PlayerStateFactory factory) : base(ctx, factory) { }

        public override void EnterState()
        {
            Debug.Log("Entering Idle State");
        }

        public override void UpdateState()
        {
            CheckSwitchStates();

            Debug.Log("Updating Idle State");

        }

        public override void FixedUpdateState() { }

        public override void ExitState() { }

        public override void CheckSwitchStates()
        {
            if (InputManager.Movement.magnitude > 0)
            {
                Debug.Log("test");
                SwitchState(Factory.Walk());
            }
        }

        public override void InitiazeSubState() { }


        public override void OnTriggerEnter2D(Collider2D other) { }
        public override void OnTriggerStay2D(Collider2D other) { }
        public override void OnTriggerExit2D(Collider2D other) { }
    }
}