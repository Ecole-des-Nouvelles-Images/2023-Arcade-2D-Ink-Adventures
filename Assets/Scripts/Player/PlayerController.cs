using System.Collections;
using Elias.Scripts.Helper;
using Elias.Scripts.Managers;
using Noah.Scripts.Camera;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Noah.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
        [SerializeField] private Light2D playerLight;

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 7.5f;
        
        [Header("Jump")]
        [SerializeField] private float _jumpForce = 12f;
        [SerializeField] private float _jumpTime = 0.35f;
        
        [Header("Turn Check")]
        [SerializeField] private GameObject _leftLeg;
        [SerializeField] private GameObject _rightLeg;

        [Header("Ground Check")] 
        [SerializeField] private float extraHeight = 0.25f;
        [SerializeField] private LayerMask _whatIsGround;
        
        [Header("Camera")] 
        [SerializeField] private GameObject _cameraFollowGO;
        
        [HideInInspector] public bool IsFacingRight;
        [HideInInspector] public bool IsClimbing;
        [HideInInspector] public bool IsOnPlatform;
        [HideInInspector] public Rigidbody2D PlatformRb;

        public bool hasColorUpgradeR;
        public bool hasColorUpgradeG;
        public bool hasColorUpgradeB;
        
        private bool _isFalling;
        private bool _isMoving;
        private bool _isJumping;
        private float _jumpTimeCounter;
        
        private Rigidbody2D _rb;
        private Animator _anim;
        private Collider2D _coll;
        private RaycastHit2D _groundHit;
        
        private float _moveInputx;
        private float _moveInputy;
        
        private Coroutine _resetTriggerCoroutine;

        private CameraFollowObject _cameraFollowObject;
        private float _fallSpeedYDampingChangeThreshold;

        private float _normalGravity;
        private bool _isGrounded;

        public GameObject _movableBox;
        public bool _canMoveBox;
        public Rigidbody2D _movableRigidbody2D;
        public RelativeJoint2D _relativeJoint2D;
        
        private float idleTimer = 0f;
        private float idleThreshold = 5f;

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            _anim = GetComponentInChildren<Animator>();
            _rb = GetComponent<Rigidbody2D>(); 
            _relativeJoint2D = GetComponent<RelativeJoint2D>();
            //     _anim = GetComponent<Animator>();
            _coll = GetComponent<Collider2D>();
            _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();
            StartDirectionCheck();
            _fallSpeedYDampingChangeThreshold = CameraManager.Instance._fallSpeedYDampingChangeThreshold;
        }

        private void FixedUpdate()
        {
            Climb();
            Move();
        }

        private void Update()
        {
            CheckInputV2();
            Jump();
            GrabBox();
            ReleaseBox();

            if (_rb.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.Instance.IsLerpingYDamping && !CameraManager.Instance.LerpedFromPlayerFalling)
            {
                CameraManager.Instance.LerpYDamping(true);
            }

            if (_rb.velocity.y >= 0f && !CameraManager.Instance.IsLerpingYDamping && CameraManager.Instance.LerpedFromPlayerFalling)
            {
                CameraManager.Instance.LerpedFromPlayerFalling = false;
                CameraManager.Instance.LerpYDamping(false);
            }
            
    //        _anim.SetBool("IsWalking", _moveInputx != 0);
            
   //         _anim.SetBool("IsJumping", _isJumping);
            
    //        _anim.SetBool("IsFalling", _isFalling);
            
      //      _anim.SetBool("IsClimbing", IsClimbing);
            
      //      _anim.SetBool("IsFalling", !_isJumping && !_isGrounded);

            idleTimer += Time.deltaTime;
            if (idleTimer >= idleThreshold)
            {
      //          _anim.SetBool("IsDancing",true);
            }

            if (_moveInputx != 0 || _isJumping)
            {
                idleTimer = 0;
     //           _anim.SetBool("IsDancing",false);
            }
            
        }
        
        #region Jump Function
        private void Jump()
        {
            if (InputManager.instance.JumpJustPressed && (_isGrounded || IsClimbing)) 
            {
                _isJumping = true;
                _jumpTimeCounter = _jumpTime;
                _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
                
    //            _anim.SetTrigger("jump");
            }

            if (InputManager.instance.JumpBeingHeld)
            {
                if (_jumpTimeCounter > 0 && _isJumping)
                {
                    _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
                    _jumpTimeCounter -= Time.deltaTime;
                }

                else if (_jumpTimeCounter == 0)
                {
                    _isFalling = true;
                    _isJumping = false;
                }

                else
                {
                    _isJumping = false;
                }
                
            }
            
            if (InputManager.instance.JumpReleased)
            {
                _isJumping = false;
                _isFalling = true;
            }

            if (!_isJumping && CheckForLand())
            {
    //            _anim.SetTrigger("land");
                _resetTriggerCoroutine = StartCoroutine(Reset());
            }
        }
        #endregion
        
        #region Movement Functions
        private void Move()
        {
            _moveInputx = InputManager.instance.MoveInput.x;
            if (_moveInputx > 0 || _moveInputx < 0)
            {
                TurnCheck();
            }
            if (IsOnPlatform)
            {
                if (IsMoving())
                {
                    _rb.velocity = new Vector2(_moveInputx * _moveSpeed, _rb.velocity.y);
                }
                else if (IsMoving() == false)
                { 
                    _rb.velocity = new Vector2(_moveInputx * _moveSpeed + PlatformRb.velocity.x, _rb.velocity.y);
                }
            }
            else
            {
                _rb.velocity = new Vector2(_moveInputx * _moveSpeed, _rb.velocity.y);
            }
        }

        private bool IsMoving()
        {
            if (_rb.velocity.x != 0 && _rb.velocity.x != PlatformRb.velocity.x) 
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Ground/Landed + Push/Pull Functions
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (Tags.CompareTags("Movable", other.gameObject))
            {
                _canMoveBox = true;
                _movableBox = other.gameObject;
            }        
            
            if (other.CompareTag("Movable,Ground"))
            {
                _isGrounded = true;
            }
            
            if (other.CompareTag("Ground"))
            {
                _isGrounded = true;
            }
        }
        

        private void OnTriggerExit2D(Collider2D other)
        {
            if (Tags.CompareTags("Movable", other.gameObject))
            {
                _canMoveBox = false;
            }        
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (Tags.CompareTags("Ground", other.gameObject))
            {
                _isGrounded = false;
            }
        }

        private void GrabBox()
        {
           if (_isGrounded)
           {
                    if (InputManager.instance.PushPullBeingHeld && _canMoveBox && _movableBox != null)
                    {
                        _movableRigidbody2D = _movableBox.GetComponent<Rigidbody2D>();

                        if (_movableRigidbody2D != null)
                        {
                            // Freeze all constraints
                            _movableRigidbody2D.constraints = ~RigidbodyConstraints2D.FreezeAll;

                            // Freeze rotation on the Z-axis
                            _movableRigidbody2D.constraints |= RigidbodyConstraints2D.FreezeRotation;

                            _relativeJoint2D.enabled = true;
                            _relativeJoint2D.connectedBody = _movableRigidbody2D;

                            // Determine if pushing or pulling
                            if (_movableRigidbody2D.velocity.x > 0)
                            {
                                // Play pushing animation
                                if (IsFacingRight)
                                {
                                    _anim.SetBool("IsPulling", false);
                                    _anim.SetBool("IsPushing", true);
                                    
                                    
                                }
                                else
                                {
                                    _anim.SetBool("IsPushing", false);
                                    _anim.SetBool("IsPulling", true);
                                    
                                }
                            }
                            else if (_movableRigidbody2D.velocity.x < 0)
                            {
                                if (IsFacingRight)
                                {
                                    _anim.SetBool("IsPushing", false);
                                    _anim.SetBool("IsPulling", true);
                                    
                                }
                                else
                                {
                                    _anim.SetBool("IsPulling", false);
                                    _anim.SetBool("IsPushing", true);
                                    
                                }
                            }
                        }
                    }
           }
           else if (!_isGrounded)
           {
               if (_relativeJoint2D != null)
               {
                   _relativeJoint2D.enabled = false;
                   _relativeJoint2D.connectedBody = null;
               }
           }
        }

        public void ReleaseBox()
        {
            if (InputManager.instance.PushPullReleased && _movableBox != null)
            {
                _movableRigidbody2D = _movableBox.GetComponent<Rigidbody2D>();
                if (_movableRigidbody2D != null)
                {
                    _movableRigidbody2D.constraints = ~RigidbodyConstraints2D.FreezeAll;
                    _movableRigidbody2D.constraints = ~RigidbodyConstraints2D.FreezePositionY;
                    
                    if (_relativeJoint2D != null)
                    {
                        _relativeJoint2D.enabled = false;
                        _relativeJoint2D.connectedBody = null;
                    }
                }
                _anim.SetBool("IsPushing", false);
                _anim.SetBool("IsPulling", false);
            }
        }

        
        private bool CheckForLand()
        {
            if (_isFalling)
            {
                if (_isGrounded)
                {
                    _isFalling = false;
                    return true;
                }
                
                return false;
                    
            }
            return false;
        }

        private IEnumerator Reset()
        {
            yield return null;
            
//            _anim.ResetTrigger("land");
        }
        #endregion

        #region Turn Checks
        private void StartDirectionCheck()
        {
            if (_rightLeg.transform.position.x > _leftLeg.transform.position.x)
            {
                IsFacingRight = true;
            }
        
            else
            {
                IsFacingRight = false;
            }
        }

        private void TurnCheck()
        {
            if (InputManager.instance.MoveInput.x > 0 && !IsFacingRight)
            {
                Turn();
            }
            
            else if (InputManager.instance.MoveInput.x < 0 && IsFacingRight)
            {
                Turn();
            }
        }

        private void Turn()
        {
            if (IsFacingRight)
            {
                Vector3 rotator = new Vector3(0f, 180f, 0f);
                transform.rotation = Quaternion.Euler(rotator);
                IsFacingRight = !IsFacingRight;
                _cameraFollowObject.CallTurn();
            }

            else
            {
                Vector3 rotator = new Vector3(0f, 0f, 0f);
                transform.rotation = Quaternion.Euler(rotator);
                IsFacingRight = !IsFacingRight;
                _cameraFollowObject.CallTurn();
            }
        }
        #endregion
        
        #region Climb Function
        private void Climb()
        {
            _moveInputx = InputManager.instance.MoveInput.x;
            _moveInputy = InputManager.instance.MoveInput.y;

            if (IsClimbing)
            { 
                _rb.velocity = new Vector2(_moveInputx * _moveSpeed, _moveInputy * _moveSpeed);
                _rb.gravityScale = 0f;
            }

            else
            {
                _rb.gravityScale = 7f;
            }
        }
        #endregion
        
        /*private void DrawGroundCheck()
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
            
            Debug.DrawRay(_coll.bounds.center + new Vector3(_coll.bounds.extents.x, 0), Vector2.down * (_coll.bounds.extents.y + extraHeight), rayColor);
            Debug.DrawRay(_coll.bounds.center - new Vector3(_coll.bounds.extents.x, 0), Vector2.down * (_coll.bounds.extents.y + extraHeight), rayColor);
            Debug.DrawRay(
                _coll.bounds.center - new Vector3(_coll.bounds.extents.x, _coll.bounds.extents.y + extraHeight),
                Vector2.right * (_coll.bounds.extents.x * 2), rayColor);
        }*/
        
        
        private void CheckInput()
        {
            if (!Input.anyKeyDown)
                return;
            
            foreach (KeyCode key in InputHelpers.InputList)
            {
                if (Input.GetKeyDown(key))
                {
                    switch (key)
                    {
                        case KeyCode.R:
                            ChangeColor(GetColor(KeyCode.G, Color.yellow, KeyCode.B, Color.magenta, Color.red));
                            break;

                        case KeyCode.G:
                            ChangeColor(GetColor(KeyCode.B, Color.cyan, KeyCode.R, Color.yellow, Color.green));
                            break;

                        case KeyCode.B:
                            ChangeColor(GetColor(KeyCode.R, Color.magenta, KeyCode.G, Color.cyan, Color.blue));
                            break;
                    }
                }
            }

            Color GetColor(KeyCode secondKey, Color colorIfBothPressed, KeyCode thirdKey, Color colorIfThirdPressed, Color defaultColor)
            {
                if (Input.GetKey(secondKey))
                    return colorIfBothPressed;
                else if (Input.GetKey(thirdKey))
                    return colorIfThirdPressed;
                else
                    return defaultColor;
            }
        } 
        public void ChangeColor(Color newColor)
        {
            playerLight.color = newColor;
        }

        private void CheckInputV2()
        {
            if (InputManager.instance.RedLightJustPressed && hasColorUpgradeR)
            {
                ChangeColor(GetColor(InputManager.instance.GreenLightBeingHeld, Color.yellow, InputManager.instance.BlueLightBeingHeld, Color.magenta, Color.red));
            }

            if (InputManager.instance.GreenLightJustPressed && hasColorUpgradeG)
            {
                ChangeColor(GetColor(InputManager.instance.BlueLightBeingHeld, Color.cyan, InputManager.instance.RedLightBeingHeld, Color.yellow, Color.green));
            }
            
            if (InputManager.instance.BlueLightJustPressed && hasColorUpgradeB)
            {
                ChangeColor(GetColor(InputManager.instance.RedLightBeingHeld, Color.magenta, InputManager.instance.GreenLightBeingHeld, Color.cyan, Color.blue));
            }

            Color GetColor(bool secondKey, Color colorIfBothPressed, bool thirdKey, Color colorIfThirdPressed, Color defaultColor)
            {
                if (secondKey)
                    return colorIfBothPressed; 
                if (thirdKey)
                    return colorIfThirdPressed;
                
                return defaultColor;
            }
        }
    }
        
        
}

