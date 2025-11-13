using UnityEngine;
using Input;

namespace FSM
{
    public class PlayerGroundedState : PlayerBaseState, IRootState
    {
        public PlayerGroundedState(PlayerStateMachine ctx, PlayerStateFactory factory) : base(ctx, factory)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            InitializeSubState();
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void FixedUpdateState()
        {
            if (_currentSubState == null) return;
            Ctx.Move(Ctx.MovementStats.GroundAcceleration, Ctx.MovementStats.GroundDeceleration, InputManager.Movement);
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
            if (InputManager.JumpWasPressed && Ctx.IsGrounded)
            {
                SwitchState(Factory.InAir());
                return;
            }

            if (InputManager.BlueLightButtonWasPressed)
            {
                SwitchState(Factory.Test());
            }
        }


        public override void InitializeSubState()
        {
            if (InputManager.Movement.magnitude == 0)
                SetSubState(Factory.Idle());
            else
                SetSubState(Factory.Walk());
        }

        public override void OnTriggerEnter2D(Collider2D other) { }
        public override void OnTriggerStay2D(Collider2D other) { }
        public override void OnTriggerExit2D(Collider2D other) { }

        public void HandleGravity()
        {
        }
    }
}
