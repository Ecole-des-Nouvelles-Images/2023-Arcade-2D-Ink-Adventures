using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputManager : MonoBehaviour
    {
        public static PlayerInput PlayerInput;

        public static Vector2 Movement;
        public static Vector2 Climb;

        public static bool JumpWasPressed;
        // public static bool JumpIsHeld;
        public static bool JumpWasReleased;
        public static bool RedLightButtonWasPressed;
        public static bool BlueLightButtonWasPressed;
        public static bool GreenLightButtonWasPressed;
        public static bool RedLightButtonIsHeld;
        public static bool BlueLightButtonIsHeld;
        public static bool GreenLightButtonIsHeld;


        private InputAction _moveAction;
        private InputAction _climbAction;
        private InputAction _jumpAction;
        private InputAction _redLightAction;
        private InputAction _blueLightAction;
        private InputAction _greenLightAction;
        
        //Elias Wheel
        
        private InputAction _openWheelAction;

        public static bool OpenWheelWasPressed;
        public static bool OpenWheelIsHeld;
        public static bool OpenWheelWasReleased;


        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
            
            _moveAction = PlayerInput.actions["Move"];
            _climbAction = PlayerInput.actions["Climb"];
            _jumpAction = PlayerInput.actions["Jump"];
            _redLightAction = PlayerInput.actions["RedLight"];
            _blueLightAction = PlayerInput.actions["BlueLight"];
            _greenLightAction = PlayerInput.actions["GreenLight"];
            
            _openWheelAction = PlayerInput.actions["OpenWheel"]; // Elias wheel

        }

        private void Update()
        {
            Movement = _moveAction.ReadValue<Vector2>();
            Climb = _climbAction.ReadValue<Vector2>();

            JumpWasPressed = _jumpAction.WasPressedThisFrame();
            // JumpIsHeld = _jumpAction.IsPressed();
            JumpWasReleased = _jumpAction.WasReleasedThisFrame();
            RedLightButtonWasPressed = _redLightAction.WasPressedThisFrame();
            BlueLightButtonWasPressed = _blueLightAction.WasPressedThisFrame();
            GreenLightButtonWasPressed = _greenLightAction.WasPressedThisFrame();
            RedLightButtonIsHeld = _redLightAction.IsInProgress();
            BlueLightButtonIsHeld = _blueLightAction.IsInProgress();
            GreenLightButtonIsHeld = _greenLightAction.IsInProgress();
            
            // Elias wheel
            OpenWheelWasPressed = _openWheelAction.WasPressedThisFrame();
            OpenWheelIsHeld = _openWheelAction.IsInProgress();
            OpenWheelWasReleased = _openWheelAction.WasReleasedThisFrame();

        }
        
    }
}
