using System;
using Input;
using Player;
using UnityEngine;

namespace FSM
{
    public class PlayerStateMachine : MonoBehaviour
    {
        private PlayerBaseState _currentState;
        private PlayerStateFactory _states;

        [Header("References")]
        public PlayerMovementStats MovementStats;
        [SerializeField] private Collider2D _feetCollider;
        [SerializeField] private Collider2D _bodyCollider;



        private Rigidbody2D _rb;
        private Animator _animator;
        private bool _isGrounded;
        private RaycastHit2D _groundHit;
        private bool _isFacingRight;

        public Rigidbody2D Rb => _rb;
        public Vector2 MoveVelocity { get ; set; }
        public float CurrentSpeed { get; set; }
        public float CurrentDeceleration { get; set; }
        public bool IsGrounded => _isGrounded;
        public PlayerBaseState CurrentState { get; set; }

        private void Awake()
        {
            _states = new PlayerStateFactory(this);

            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            _isFacingRight = true;
            _currentState = _states.Grounded();
            _currentState.EnterState();
        }

        private void Update()
        {
            _currentState.UpdateStates();
            Debug.Log("Current State : " +_currentState);
            // Debug.Log("Current SubState : " +_currentState._currentSubState);
        }

        private void FixedUpdate()
        {
            CollisionChecks();
            _currentState.FixedUpdateStates();

            _animator.SetFloat("xVelocity", Math.Abs(_rb.velocity.x));
            _animator.SetFloat("yVelocity", Math.Abs(_rb.velocity.y));
        }

        private void CollisionChecks()
        {
            HandleGroundCheck();
        }

        private void HandleGroundCheck()
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

        public void TurnCheck(Vector2 moveInput)
        {
            if (_isFacingRight && moveInput.x < 0)
            {
                Turn(false);
                Debug.Log("Facing Right");
            }

            else if (!_isFacingRight && moveInput.x > 0)
            {
                Turn(true);
                Debug.Log("Facing Left");
            }
        }

        private void Turn(bool turnRight)
        {
            _isFacingRight = turnRight;

            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (turnRight ? 1 : -1);
            transform.localScale = localScale;
        }
    }
}
