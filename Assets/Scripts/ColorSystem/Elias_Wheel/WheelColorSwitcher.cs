using System.Collections.Generic;
using UnityEngine;

namespace ColorSystem.Elias_Wheel
{
    public class WheelColorSwitcher : MonoBehaviour
    {
        private PlayerColorManager _colorManager;
        private bool _isWheelOpen;
        private Color _selectedColor = Color.clear;

        private readonly List<Color> _wheelColors = new()
        {
            Color.red,
            Color.magenta,
            Color.blue,
            Color.cyan,
            Color.green,
            Color.yellow
        };

        private void Start()
        {
            _colorManager = GetComponent<PlayerColorManager>();
        }

        private void Update()
        {
            HandleWheelInput();
        }

        private void HandleWheelInput()
        {
            if (Input.InputManager.OpenWheelWasPressed)
            {
                OpenWheel();
            }

            if (Input.InputManager.OpenWheelIsHeld)
            {
                UpdateSelection();
            }

            if (Input.InputManager.OpenWheelWasReleased)
            {
                ConfirmSelection();
                CloseWheel();
            }
        }

        private void OpenWheel()
        {
            _isWheelOpen = true;
            _selectedColor = Color.clear;
            // TODO Afficher l'UI
        }

        private void CloseWheel()
        {
            _isWheelOpen = false;
            // TODO Effacer l'UI
        }

        private void UpdateSelection()
        {
            Vector2 stickInput = Input.InputManager.Movement;
            if (stickInput.magnitude < 0.2f) return; // Deadzone

            float angle = Mathf.Atan2(stickInput.y, stickInput.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            int sliceIndex = Mathf.FloorToInt(angle / 60f);
            sliceIndex = Mathf.Clamp(sliceIndex, 0, _wheelColors.Count - 1);

            Color candidate = _wheelColors[sliceIndex];

            if (_colorManager != null &&
                (_colorManager.SwitchableColors.Contains(candidate) ||
                 _colorManager.CanMixColorPublic(candidate)))
            {
                _selectedColor = candidate;
                // TODO Highlighter L'UI sélectionnée
            }
        }

        private void ConfirmSelection()
        {
            if (_selectedColor != Color.clear && _colorManager != null)
            {
                _colorManager.ChangeColor(_selectedColor);
            }
        }
    }
}
