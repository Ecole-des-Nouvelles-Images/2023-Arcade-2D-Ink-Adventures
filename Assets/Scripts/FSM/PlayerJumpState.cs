using UnityEngine;

namespace FSM
{
    public class PlayerJumpState : PlayerInAirState
    {
        public PlayerJumpState(PlayerStateMachine ctx, PlayerStateFactory factory) : base(ctx, factory)
        {
            IsRootState = false;
        }

        public override void EnterState()
        {
            IsJumping = true;
            _jumpBufferTimer = 0f;
            VerticalVelocity = Ctx.MovementStats.InitialJumpVelocity;
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
            IsJumping = false;
        }

        public override void CheckSwitchStates()
        {
            if (VerticalVelocity <= 0f)
            {
                SwitchState(Factory.Fall());
            }
        }

        public override void InitializeSubState() { }
        public override void OnTriggerEnter2D(Collider2D other) { }
        public override void OnTriggerStay2D(Collider2D other) { }
        public override void OnTriggerExit2D(Collider2D other) { }
    }
}