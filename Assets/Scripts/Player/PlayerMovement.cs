using System;
using Input;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public static PlayerMovement Instance;
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
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _isFacingRight = true;
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            CollisionChecks();
            Jump();

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
            JumpChecks();
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

            else if (_jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < MovementStats.NumberOfJumpsAllowed - 1)
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
                    }

                    // GRAVITY ON ASCENDING NOT PAST APEX THRESHOLD
                    else
                    {
                        VerticalVelocity += MovementStats.Gravity * Time.fixedDeltaTime;
                        if (_isPastApexThreshold)
                        {
                            _isPastApexThreshold = false;
                        }
                    }
                }
                
                // GRAVITY ON DESCENDING
                else if (!_isFastFalling)
                {
                    VerticalVelocity += MovementStats.Gravity * MovementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }
                else if(VerticalVelocity < 0)
                {
                    if (!_isFalling)
                    {
                        _isFalling = true;
                    }
                }
            }

            // JUMP CUT
            if (_isFastFalling)
            {
                if (_fastFallTime >= MovementStats.TimeForUpwardsCancel)
                {
                    VerticalVelocity += MovementStats.Gravity * MovementStats.GravityOnReleaseMultiplier *
                                        Time.fixedDeltaTime;
                }
                else if (_fastFallTime < MovementStats.TimeForUpwardsCancel)
                {
                    VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f,
                        (_fastFallTime / MovementStats.TimeForUpwardsCancel));
                }

                _fastFallTime += Time.fixedDeltaTime;
            }

            if (!_isGrounded && !_isJumping)
            {
                if (!_isFastFalling)
                {
                    _isFalling = true;
                }
                
                VerticalVelocity += MovementStats.Gravity * Time.fixedDeltaTime;
            }

            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MovementStats.MaxFallSpeed, 50f);

            _rb.velocity = new Vector2(_rb.velocity.x, VerticalVelocity);
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
                Debug.DrawRay(bottomLeft - Vector2.down * MovementStats.GroundDetectionRayLength,
                    Vector2.right * boxCastSize.x, rayColor);
            }

            #endregion
        }

        private void BumpedHead()
        {
            // Ajustez l'origine du BoxCast pour qu'il soit au sommet de la tête
            Vector2 boxCastOrigin = new Vector2(_bodyCollider.bounds.center.x, _bodyCollider.bounds.max.y);

            // Ajustez la taille du BoxCast pour correspondre à la largeur de la tête
            Vector2 boxCastSize = new Vector2(_bodyCollider.bounds.size.x * MovementStats.HeadWidth, MovementStats.HeadDetectionRayLength);

            _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up,
                MovementStats.HeadDetectionRayLength, MovementStats.GroundLayer);

            if (_headHit.collider != null)
            {
                _bumpedHead = true;
            }
            else
            {
                _bumpedHead = false;
            }

            #region Debug Visualization

            if (MovementStats.DebugShowHeadBumpBox)
            {
                float headWidth = MovementStats.HeadWidth;
                Color rayColor = _bumpedHead ? Color.green : Color.red;

                // Dessinez les lignes de débogage pour visualiser la boîte de détection
                Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y),
                    Vector2.up * MovementStats.HeadDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y),
                    Vector2.up * MovementStats.HeadDetectionRayLength, rayColor);
                Debug.DrawRay(
                    new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth,
                        boxCastOrigin.y + MovementStats.HeadDetectionRayLength),
                    Vector2.right * boxCastSize.x * headWidth, rayColor);
            }

            #endregion
        }

        private void DrawJumpArc(float moveSpeed, Color gizmoColor)
        {
            Vector2 startPosition = new Vector2(_feetCollider.bounds.center.x, _feetCollider.bounds.min.y);
            Vector2 previousPosition = startPosition;
            float speed = 0f;
            if (MovementStats.DrawRight)
            {
                speed = moveSpeed;
            }
            else
            {
                speed = -moveSpeed;
            }

            Vector2 velocity = new Vector2(speed, MovementStats.InitialJumpVelocity);

            Gizmos.color = gizmoColor;

            float timeStep = 2 * MovementStats.TimeTillJumpApex / MovementStats.ArcResolution;

            for (int i = 0; i < MovementStats.VisualizationSteps; i++)
            {
                float simulationTime = i * timeStep;
                Vector2 displacement;
                Vector2 drawPoint;

                if (simulationTime < MovementStats.TimeTillJumpApex)
                {
                    displacement = velocity * simulationTime +
                                   0.5f * new Vector2(0, MovementStats.Gravity) * simulationTime * simulationTime;
                }
                else if (simulationTime < MovementStats.TimeTillJumpApex + MovementStats.ApexHangTime)
                {
                    float apexTime = simulationTime - MovementStats.TimeTillJumpApex;
                    displacement = velocity * MovementStats.TimeTillJumpApex + 0.5f *
                        new Vector2(0, MovementStats.Gravity) * MovementStats.TimeTillJumpApex *
                        MovementStats.TimeTillJumpApex;
                    displacement += new Vector2(speed, 0) * apexTime;
                }
                else
                {
                    float descendTime =
                        simulationTime - (MovementStats.TimeTillJumpApex + MovementStats.ApexHangTime);
                    displacement = velocity * MovementStats.TimeTillJumpApex + 0.5f *
                        new Vector2(0, MovementStats.Gravity) * MovementStats.TimeTillJumpApex *
                        MovementStats.TimeTillJumpApex;
                    displacement += new Vector2(speed, 0) * MovementStats.ApexHangTime;
                    displacement += new Vector2(speed, 0) * descendTime +
                                    0.5f * new Vector2(0, MovementStats.Gravity) * descendTime * descendTime;
                }

                drawPoint = startPosition + displacement;

                if (MovementStats.StopOnCollision)
                {
                    RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition,
                        Vector2.Distance(drawPoint, previousPosition), MovementStats.GroundLayer);
                    if (hit.collider != null)
                    {
                        Gizmos.DrawLine(previousPosition, hit.point);
                        break;
                    }
                }

                Gizmos.DrawLine(previousPosition, drawPoint);
                previousPosition = drawPoint;
            }
        }

        private void OnDrawGizmos()
        {
            if (MovementStats.ShowWalkJumpArc)
            {
                DrawJumpArc(MovementStats.MaxWalkSpeed, Color.white);
            }
        }

        private void CollisionChecks()
        {
            IsGrounded();
            BumpedHead();
        }

        #endregion
    }
}