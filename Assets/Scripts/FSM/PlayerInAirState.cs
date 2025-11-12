using Input;
using UnityEngine;

namespace FSM
{
    public class PlayerInAirState : PlayerBaseState, IRootState
    {
        public PlayerInAirState(PlayerStateMachine ctx, PlayerStateFactory factory) : base(ctx, factory)
        {
            IsRootState = true;
        }

        protected float VerticalVelocity { get; set; }
        protected bool IsJumping { get; set; }
        protected bool IsFalling { get; set; }
        protected bool IsFastFalling { get; set; }

        private float _fastFallTime;
        private float _fastFallReleaseSpeed;

        private float _apexPoint;
        private float _timePastApexThreshold;
        private bool _isPastApexThreshold;

        protected float _jumpBufferTimer;
        private bool _jumpReleasedDuringBuffer;
        private float _coyoteTimer;
        private int _numberOfJumpsUsed;

        public override void EnterState()
        {
            InitializeSubState();
            InitiateJump();
        }

        public override void UpdateState()
        {
            Debug.Log("IsJumping = " +IsJumping);
            Debug.Log("IsFalling = " +IsFalling);
            Debug.Log("IsFastFalling = " +IsFastFalling);

            CheckSwitchStates();
        }

        public override void FixedUpdateState()
        {
            if (_currentSubState == null) return;

            Move(Ctx.MovementStats.AirAcceleration, Ctx.MovementStats.AirDeceleration, InputManager.Movement);

            _currentSubState.FixedUpdateStates();

            // HandleGravity();
        }

        public override void ExitState()
        {
            throw new System.NotImplementedException();
        }

        public override void CheckSwitchStates()
        {
            if (Ctx.IsGrounded && VerticalVelocity <= 0f)
            {
                SwitchState(Factory.Grounded());
            }

        }

        public override void InitializeSubState()
        {
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

        private void InitiateJump()
        {
            if (!IsJumping)
            {
                IsJumping = true;
            }

            _jumpBufferTimer = 0f;
            VerticalVelocity = Ctx.MovementStats.InitialJumpVelocity;
        }

        private void Move(float acceleration, float deceleration, Vector2 moveInput)
        {
            if (moveInput != Vector2.zero)
            {
                Ctx.TurnCheck(moveInput);
                Vector2 targetVelocity = Vector2.zero;
                targetVelocity = new Vector2(moveInput.x, 0f) * Ctx.MovementStats.MaxFallSpeed;

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
            throw new System.NotImplementedException();
        }
    }
}
