using UnityEngine;
using Input;

namespace FSM
{
    public class PlayerWalkState : PlayerBaseState
    {
        public PlayerWalkState(PlayerStateMachine ctx, PlayerStateFactory factory) : base(ctx, factory) { }

        public override void EnterState()
        {
            Debug.Log("Entering Walk State");
        }

        public override void UpdateState()
        {
            CheckSwitchStates();

            Debug.Log("Updating Walk State");

        }

        public override void FixedUpdateState() { }

        public override void ExitState() { }

        public override void CheckSwitchStates()
        {
            if (InputManager.Movement.magnitude == 0)
                SwitchState(Factory.Idle());
        }

        public override void InitiazeSubState() { }


        public override void OnTriggerEnter2D(Collider2D other) { }
        public override void OnTriggerStay2D(Collider2D other) { }
        public override void OnTriggerExit2D(Collider2D other) { }
    }
}