using System;
using Input;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")] public PlayerMovementStats MovementStats;
        [SerializeField] private Collider2D _feetCollider;
        [SerializeField] private Collider2D _bodyCollider;

        private Rigidbody2D _rb;

        private Vector2 _moveVelocity;
        private bool _isFacingRight;

        private RaycastHit2D _groundHit;
        private RaycastHit2D _headHit;
        private bool _isGrounded;
        private bool _bumpedHead;
        
        public float VerticalVelocity { get; private set; }
        private bool _isJumping;
        private bool _isFastFalling;
        private bool _isFalling;
        private float _fastFallTime;
        private float _fastFallReleaseSpeed;
        private int _numberOfJumpsUsed;

        private float _apexPoint;
        private float _timePastApexThreshold;
        private bool _isPastApexThreshold;

        private float _jumpBufferTimer;
        private bool _jumpReleasedDuringBuffer;
        
        private float _coyoteTimer;

        private void Awake()
        {
            _isFacingRight = true;

            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            CollisionChecks();

            if (_isGrounded)
            {
                Move(MovementStats.GroundAcceleration, MovementStats.GroundDeceleration, InputManager.Movement);
            }
            else
            {
                Move(MovementStats.AirAcceleration, MovementStats.AirDeceleration, InputManager.Movement);
            }
        }

        private void Update()
        {
            CountTimers();
            Jump();
        }

        #region Movement

        private void Move(float acceleration, float deceleration, Vector2 moveInput)
        {
            if (moveInput != Vector2.zero)
            {
                TurnCheck(moveInput);
                Vector2 targetVelocity = Vector2.zero;
                /*  IF RUN OPTION IS ADDED
                 * if (InputManager.RunIsHeld)
                 * {
                 *      targetVelocity = new Vector2(moveInput.x, 0f) * MovementStats.MaxRunSpeed;
                 * }
                 * else
                 */
                targetVelocity = new Vector2(moveInput.x, 0f) * MovementStats.MaxWalkSpeed;

                _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
                _rb.velocity = new Vector2(_moveVelocity.x, _rb.velocity.y);
            }
            else if (moveInput == Vector2.zero)
            {
                _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
                _rb.velocity = new Vector2(_moveVelocity.x, _rb.velocity.y);
            }
        }

        private void TurnCheck(Vector2 moveInput)
        {
            if (_isFacingRight && moveInput.x < 0)
            {
                Turn(false);
            }

            else if (!_isFacingRight && moveInput.x > 0)
            {
                Turn(true);
            }
        }

        private void Turn(bool turnRight)
        {
            if (turnRight)
            {
                _isFacingRight = true;
                transform.Rotate(0f, 180f, 0f);
            }
            else
            {
                _isFacingRight = false;
                transform.Rotate(0f, -180f, 0f);
            }
        }

        #endregion

        #region Jump

        private void JumpChecks()
        {
            if (InputManager.JumpWasPressed)
            {
                _jumpBufferTimer = MovementStats.JumpBufferTime;
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
                        _fastFallTime = MovementStats.TimeForUpwardsCancel;
                        VerticalVelocity = 0f;
                    }
                    else
                    {
                        _isFastFalling = true;
                        _fastFallReleaseSpeed = VerticalVelocity;
                    }
                }
            }

            if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
            {
                InitiateJump(1);

                if (_jumpReleasedDuringBuffer)
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }

            else if (_jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < MovementStats.NumberOfJumpsAllowed)
            {
                _isFastFalling = false;
                InitiateJump(1);
            }

            else if (_jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < MovementStats.NumberOfJumpsAllowed)
            {
                InitiateJump(2);
                _isFastFalling = false;
            }

            if ((_isJumping || _isFalling) && _isGrounded && VerticalVelocity <= 0f)
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

        private void Jump()
        {
            if (_isJumping)
            {
                if (_bumpedHead)
                {
                    _isFastFalling = true;
                }
            }

            if (VerticalVelocity >= 0f)
            {
                _apexPoint = Mathf.InverseLerp(MovementStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (_apexPoint > MovementStats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < MovementStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }

                    else
                    {
                        VerticalVelocity += MovementStats.Gravity * Time.fixedDeltaTime;
                        if (_isPastApexThreshold)
                        {
                            ;
                        }
                    }
                }
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
            VerticalVelocity = MovementStats.InitialJumpVelocity;
        }
        

        #endregion

        #region Timers

        private void CountTimers()
        {
            _jumpBufferTimer -= Time.deltaTime;

            if (!_isGrounded)
            {
                _coyoteTimer -= Time.deltaTime;
            }
            else
            {
                _coyoteTimer -= MovementStats.JumpCoyoteTime;
            }
        }

        #endregion

        #region Collision Checks

        private void IsGrounded()
        {
            Vector2 boxCastOrigin = new Vector2(_feetCollider.bounds.center.x, _feetCollider.bounds.min.y);
            Vector2 boxCastSize = new Vector2(_feetCollider.bounds.size.x, MovementStats.GroundDetectionRayLength);

            _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down,
                MovementStats.GroundDetectionRayLength, MovementStats.GroundLayer);
            if (_groundHit.collider != null)
            {
                _isGrounded = true;
            }
            else
            {
                _isGrounded = false;
            }
            
            #region Debug Visualization
            if (MovementStats.DebugShowIsGroundedBox)
            {
                Color rayColor;
                if (_isGrounded)
                {
                    rayColor = Color.green;
                }
                else
                {
                    rayColor = Color.red;
                }
                // Coin bas gauche
                Vector2 bottomLeft = new Vector2(boxCastOrigin.x - (boxCastSize.x / 2), boxCastOrigin.y);
                // Coin bas droit
                Vector2 bottomRight = new Vector2(boxCastOrigin.x + (boxCastSize.x / 2), boxCastOrigin.y);
                
                // Ligne verticale gauche
                Debug.DrawRay(bottomLeft, Vector2.down * MovementStats.GroundDetectionRayLength, rayColor);
                // Ligne verticale droite
                Debug.DrawRay(bottomRight, Vector2.down * MovementStats.GroundDetectionRayLength, rayColor);
                // Ligne horizontale en bas
                Debug.DrawRay(bottomLeft - Vector2.down * MovementStats.GroundDetectionRayLength, Vector2.right * boxCastSize.x, rayColor);
            }
            #endregion
        }

        private void CollisionChecks()
        {
            IsGrounded();
        }
        #endregion
    }
}