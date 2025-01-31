using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputManager : MonoBehaviour
    {
        public static PlayerInput PlayerInput;

        public static Vector2 Movement;
        public static bool JumpWasPressed;
        public static bool JumpIsHeld;
        public static bool JumpWasReleased;

        private InputAction _moveAction;
        private InputAction _jumpAction;

        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
            
            _moveAction = PlayerInput.actions["Move"];
            _jumpAction = PlayerInput.actions["Jump"];
        }

        private void Update()
        {
            Movement = _moveAction.ReadValue<Vector2>();

            JumpWasPressed = _jumpAction.WasPressedThisFrame();
            JumpIsHeld = _jumpAction.IsPressed();
            JumpWasReleased = _jumpAction.WasReleasedThisFrame();
        }
        
    }
}
