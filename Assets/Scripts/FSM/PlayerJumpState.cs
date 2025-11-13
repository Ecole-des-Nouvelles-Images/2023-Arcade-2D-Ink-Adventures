using Input;
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
            Debug.Log("Entering Jump State");
            _jumpBufferTimer = Ctx.MovementStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
            InitiateOneJump();
        }

        public override void UpdateState()
        {
            Debug.Log("Update Jump State");
            // JumpChecks();
            CheckSwitchStates();
        }

        public override void FixedUpdateState()
        {
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
            //SwitchState(Factory.Fall());
        }

        public override void InitializeSubState() { }
        public override void OnTriggerEnter2D(Collider2D other) { }
        public override void OnTriggerStay2D(Collider2D other) { }
        public override void OnTriggerExit2D(Collider2D other) { }


        private void JumpChecks()
        {
            if (InputManager.JumpWasPressed)
            {
                _jumpBufferTimer = Ctx.MovementStats.JumpBufferTime;
                _jumpReleasedDuringBuffer = false;
            }

            if (InputManager.JumpWasReleased)
            {
                if (_jumpBufferTimer > 0f)
                {
                    _jumpReleasedDuringBuffer = true;
                }

                if (_isJumping && VerticalVelocity > 0f)
                {
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                        _isFastFalling = true;
                        _fastFallTime = Ctx.MovementStats.TimeForUpwardsCancel;
                        VerticalVelocity = 0f;
                    }
                    else
                    {
                        _isFastFalling = true;
                        _fastFallReleaseSpeed = VerticalVelocity;
                    }
                }
            }

            if (_jumpBufferTimer > 0f && !_isJumping && (Ctx.IsGrounded || _coyoteTimer > 0f))
            {
                InitiateJump(1);

                if (_jumpReleasedDuringBuffer)
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }

            else if (_jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < Ctx.MovementStats.NumberOfJumpsAllowed)
            {
                _isFastFalling = false;
                InitiateJump(1);
            }

            else if (_jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < Ctx.MovementStats.NumberOfJumpsAllowed - 1)
            {
                InitiateJump(2);
                _isFastFalling = false;
            }

            if ((_isJumping || _isFalling) && Ctx.IsGrounded && VerticalVelocity <= 0f)
            {
                _isJumping = false;
                _isFalling = false;
                _isFastFalling = false;
                _fastFallTime = 0f;
                _isPastApexThreshold = false;
                _numberOfJumpsUsed = 0;

                VerticalVelocity = Physics2D.gravity.y;
            }
        }

        private void InitiateJump(int numberOfJumpsUsed)
        {
            if (!_isJumping)
            {
                _isJumping = true;
            }

            _jumpBufferTimer = 0f;
            _numberOfJumpsUsed += numberOfJumpsUsed;
            VerticalVelocity = Ctx.MovementStats.InitialJumpVelocity;
        }

        private void InitiateOneJump()
        {
            if (!_isJumping){
                _isJumping = true;
            }
            _jumpBufferTimer = 0f;
            VerticalVelocity = Ctx.MovementStats.InitialJumpVelocity;
        }
    }
}