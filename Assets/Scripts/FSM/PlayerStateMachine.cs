using System;
using Input;
using Player;
using UnityEngine;

namespace FSM
{
    public class PlayerStateMachine : MonoBehaviour
    {
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
        private RaycastHit2D _headHit;
        private bool _bumpedHead;
        public bool BumpedHead => _bumpedHead;

        public float CoyoteTime { get; set; }
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
            CurrentState = _states.Grounded();
            CurrentState.EnterState();
        }

        private void Update()
        {
            CurrentState.UpdateStates();
            Debug.Log("Current State : " +CurrentState);
            Debug.Log("Current SubState : " +CurrentState._currentSubState);
        }

        private void FixedUpdate()
        {
            CollisionChecks();
            CurrentState.FixedUpdateStates();

            _animator.SetFloat("xVelocity", Math.Abs(_rb.velocity.x));
            _animator.SetFloat("yVelocity", Math.Abs(_rb.velocity.y));
        }

        private void CollisionChecks()
        {
            HandleGroundCheck();
            HandleBumpedHeadCheck();
        }

        #region Collision Checks

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

        private void HandleBumpedHeadCheck()
        {
            // Ajustez l'origine du BoxCast pour qu'il soit au sommet de la tête
            Vector2 boxCastOrigin = new Vector2(_bodyCollider.bounds.center.x, _bodyCollider.bounds.max.y);

            // Ajustez la taille du BoxCast pour correspondre à la largeur de la tête
            Vector2 boxCastSize = new Vector2(_bodyCollider.bounds.size.x * MovementStats.HeadWidth,
                MovementStats.HeadDetectionRayLength);

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

        public void TurnCheck(Vector2 moveInput)
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
            _isFacingRight = turnRight;

            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (turnRight ? 1 : -1);
            transform.localScale = localScale;
        }
        #endregion
    }
}
