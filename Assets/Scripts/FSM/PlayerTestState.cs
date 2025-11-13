using UnityEngine;
using Input;

namespace FSM
{
    public class PlayerTestState : PlayerBaseState
    {
        public PlayerTestState(PlayerStateMachine ctx, PlayerStateFactory factory) : base(ctx, factory)
        {
        }

        public override void EnterState()
        {
            Debug.Log("Entering Test State");
        }

        public override void UpdateState()
        {
            Debug.Log("Updating Test State");
        }

        public override void FixedUpdateState()
        {
        }

        public override void ExitState() { }

        public override void CheckSwitchStates()
        {
        }

        public override void InitializeSubState() { }
        public override void OnTriggerEnter2D(Collider2D other) { }
        public override void OnTriggerStay2D(Collider2D other) { }
        public override void OnTriggerExit2D(Collider2D other) { }
    }
}