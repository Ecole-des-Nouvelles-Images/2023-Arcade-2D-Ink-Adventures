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