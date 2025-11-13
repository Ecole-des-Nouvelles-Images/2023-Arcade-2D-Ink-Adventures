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

        private bool _bumpedHead;

        public float VerticalVelocity { get; protected set; }
        protected bool _isJumping;
        protected bool _isFastFalling;
        protected bool _isFalling;
        protected float _fastFallTime;
        protected float _fastFallReleaseSpeed;
        protected int _numberOfJumpsUsed;

        private float _apexPoint;
        private float _timePastApexThreshold;
        protected bool _isPastApexThreshold;

        protected float _jumpBufferTimer;
        protected bool _jumpReleasedDuringBuffer;

        protected float _coyoteTimer;

        public override void EnterState()
        {
            InitializeSubState();
        }

        public override void UpdateState()
        {
            CountTimers();
            CheckSwitchStates();
        }

        public override void FixedUpdateState()
        {
            HandleGravity();
            Ctx.Move(Ctx.MovementStats.AirAcceleration, Ctx.MovementStats.AirDeceleration, InputManager.Movement);
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
            if (Ctx.IsGrounded && !_isJumping)
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

        public void HandleGravity()
        {
            if (!_isJumping) return;
            if (_bumpedHead)
            {
                _isFastFalling = true;
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
            else if (!_isFastFalling)
            {
                VerticalVelocity += Ctx.MovementStats.Gravity * Ctx.MovementStats.GravityOnReleaseMultiplier *
                                    Time.fixedDeltaTime;
            }
            else if (VerticalVelocity < 0)
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                }
            }

            // JUMP CUT
            if (_isFastFalling)
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

            if (!Ctx.IsGrounded && !_isJumping)
            {
                if (!_isFastFalling)
                {
                    _isFalling = true;
                }

                VerticalVelocity += Ctx.MovementStats.Gravity * Time.fixedDeltaTime;
            }

            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -Ctx.MovementStats.MaxFallSpeed, 50f);

            Ctx.Rb.velocity = new Vector2(Ctx.Rb.velocity.x, VerticalVelocity);
        }

        private void CountTimers()
        {
            _jumpBufferTimer -= Time.deltaTime;

            if (!Ctx.IsGrounded)
            {
                _coyoteTimer -= Time.deltaTime;
            }
            else
            {
                _coyoteTimer -= Ctx.MovementStats.JumpCoyoteTime;
            }
        }
    }
}

