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

            Move(Ctx.MovementStats.GroundAcceleration, Ctx.MovementStats.GroundDeceleration, InputManager.Movement);

            _currentSubState.FixedUpdateStates();
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
            if (InputManager.JumpWasPressed && (Ctx.IsGrounded || Ctx.CoyoteTime > 0f))
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

        private void Move(float acceleration, float deceleration, Vector2 moveInput)
        {
            if (moveInput != Vector2.zero)
            {
                Ctx.TurnCheck(moveInput);
                Vector2 targetVelocity = Vector2.zero;
                targetVelocity = new Vector2(moveInput.x, 0f) * Ctx.MovementStats.MaxWalkSpeed;

                Ctx.MoveVelocity = Vector2.Lerp(Ctx.MoveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
                Ctx.Rb.velocity = new Vector2(Ctx.MoveVelocity.x, Ctx.Rb.velocity.y);
            }
            else if (moveInput == Vector2.zero)
            {
                Ctx.MoveVelocity = Vector2.Lerp(Ctx.MoveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
                Ctx.Rb.velocity = new Vector2(Ctx.MoveVelocity.x, Ctx.Rb.velocity.y);
            }
        }

        public void HandleGravity()
        {
        }
    }
}
