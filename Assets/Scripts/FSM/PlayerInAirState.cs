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


            HandleJumpBuffer();
            HandleCoyoteTime();

            CheckSwitchStates();
        }

        public override void FixedUpdateState()
        {
            if (_currentSubState == null) return;
            HandleGravity();
            Move(Ctx.MovementStats.AirAcceleration, Ctx.MovementStats.AirDeceleration, InputManager.Movement);
            _currentSubState.FixedUpdateStates();
        }

        public override void ExitState()
        {
            IsJumping = false;
            IsFalling = false;
            IsFastFalling = false;
            _numberOfJumpsUsed = 0;
            Ctx.CoyoteTime = 0f;
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
            if (InputManager.JumpWasPressed)
            {
                SetSubState(Factory.Jump());
            }
            else
            {
                SetSubState(Factory.Fall());
            }
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
            if (IsJumping)
            {
                if (Ctx.BumpedHead)
                {
                    IsFastFalling = true;
                }

                if (VerticalVelocity >= 0f)
                {
                    _apexPoint = Mathf.InverseLerp(Ctx.MovementStats.InitialJumpVelocity, 0f, VerticalVelocity);

                    if (_apexPoint > Ctx.MovementStats.ApexThreshold)
                    {
                        if (!_isPastApexThreshold)
                        {
                            _isPastApexThreshold = true;
                            _timePastApexThreshold = 0f;
                        }

                        if (_isPastApexThreshold)
                        {
                            _timePastApexThreshold += Time.fixedDeltaTime;
                            if (_timePastApexThreshold < Ctx.MovementStats.ApexHangTime)
                            {
                                VerticalVelocity = 0f;
                            }
                            else
                            {
                                VerticalVelocity = -0.01f;
                            }
                        }
                    }

                    // GRAVITY ON ASCENDING NOT PAST APEX THRESHOLD
                    else
                    {
                        VerticalVelocity += Ctx.MovementStats.Gravity * Time.fixedDeltaTime;
                        if (_isPastApexThreshold)
                        {
                            _isPastApexThreshold = false;
                        }
                    }
                }

                // GRAVITY ON DESCENDING
                else if (!IsFastFalling)
                {
                    VerticalVelocity += Ctx.MovementStats.Gravity * Ctx.MovementStats.GravityOnReleaseMultiplier *
                                        Time.fixedDeltaTime;
                }
                else if (VerticalVelocity < 0)
                {
                    if (!IsFalling)
                    {
                        IsFalling = true;
                    }
                }
            }

            // JUMP CUT
            if (IsFastFalling)
            {
                if (_fastFallTime >= Ctx.MovementStats.TimeForUpwardsCancel)
                {
                    VerticalVelocity += Ctx.MovementStats.Gravity * Ctx.MovementStats.GravityOnReleaseMultiplier *
                                        Time.fixedDeltaTime;
                }
                else if (_fastFallTime < Ctx.MovementStats.TimeForUpwardsCancel)
                {
                    VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f,
                        (_fastFallTime / Ctx.MovementStats.TimeForUpwardsCancel));
                }

                _fastFallTime += Time.fixedDeltaTime;
            }

            if (!Ctx.IsGrounded && !IsJumping)
            {
                if (!IsFastFalling)
                {
                    IsFalling = true;
                }

                VerticalVelocity += Ctx.MovementStats.Gravity * Time.fixedDeltaTime;
            }

            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -Ctx.MovementStats.MaxFallSpeed, 50f);

            Ctx.Rb.velocity = new Vector2(Ctx.Rb.velocity.x, VerticalVelocity);        }

        private void HandleJumpBuffer()
        {
            if (InputManager.JumpWasPressed)
            {
                _jumpBufferTimer = Ctx.MovementStats.JumpBufferTime;
                _jumpReleasedDuringBuffer = false;
            }

            _jumpBufferTimer -= Time.deltaTime;
        }

        private void HandleCoyoteTime()
        {
            if (!Ctx.IsGrounded)
                _coyoteTimer -= Time.deltaTime;
            else
                _coyoteTimer = Ctx.MovementStats.JumpCoyoteTime;
        }


    }
}
