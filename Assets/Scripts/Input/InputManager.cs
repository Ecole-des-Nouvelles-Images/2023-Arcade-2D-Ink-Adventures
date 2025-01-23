using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    
    public Vector2 MoveInput { get; private set; }
    public bool JumpJustPressed { get; private set; }
    public bool JumpBeingHeld { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool MenuOpenCloseInput { get; private set; }
    public bool PushPullBeingHeld { get; private set; }
    public bool PushPullReleased { get; private set; }
    public bool RedLightBeingHeld { get; private set; }
    public bool BlueLightBeingHeld { get; private set; }
    public bool GreenLightBeingHeld { get; private set; }
    public bool RedLightJustPressed { get; private set; }
    public bool BlueLightJustPressed { get; private set; }
    public bool GreenLightJustPressed { get; private set; }



    
    private PlayerInput _playerInput;
    
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _menuOpenCloseAction;
    private InputAction _pushPullAction;
    private InputAction _redLightAction;
    private InputAction _blueLightAction;
    private InputAction _greenLightAction;




    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _playerInput = GetComponent<PlayerInput>();
        SetUpActions();
    }

    private void Update()
    {
        UpdateInputs();
    }

    private void SetUpActions()
    {
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _pushPullAction = _playerInput.actions["PushPull"];
        _menuOpenCloseAction = _playerInput.actions["MenuOpenClose"];
        _redLightAction = _playerInput.actions["RedLight"];
        _blueLightAction = _playerInput.actions["BlueLight"];
        _greenLightAction = _playerInput.actions["GreenLight"];

    }
    
    private void UpdateInputs()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        JumpJustPressed = _jumpAction.WasPressedThisFrame();
        JumpBeingHeld = _jumpAction.IsPressed();
        JumpReleased = _jumpAction.WasReleasedThisFrame();
        MenuOpenCloseInput = _menuOpenCloseAction.WasPressedThisFrame();
        PushPullBeingHeld = _pushPullAction.IsPressed();
        PushPullReleased = _pushPullAction.WasReleasedThisFrame();
        RedLightJustPressed = _redLightAction.WasPerformedThisFrame();
        BlueLightJustPressed = _blueLightAction.WasPerformedThisFrame();
        GreenLightJustPressed = _greenLightAction.WasPerformedThisFrame();
        RedLightBeingHeld = _redLightAction.IsInProgress();
        BlueLightBeingHeld = _blueLightAction.IsInProgress();
        GreenLightBeingHeld = _greenLightAction.IsInProgress();
    }
}
