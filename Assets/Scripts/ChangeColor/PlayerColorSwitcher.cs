using Input;
using UnityEngine;

namespace ChangeColor
{
    public class PlayerColorSwitcher : MonoBehaviour
    {
        public PlayerColorManager _colorManager;

        private void Start()
        {
            _colorManager = GetComponent<PlayerColorManager>();
            if (_colorManager == null)
            {
                Debug.LogError("PlayerColorManager component not found.");
            }
        }


        private void Update()
        {
            HandleColorSwitchInput();
        }

        private void HandleColorSwitchInput()
        {
            if (InputManager.RedLightButtonWasPressed)
            {
                TryChangeColor(Color.red,InputManager.GreenLightButtonIsHeld, Color.yellow, InputManager.BlueLightButtonIsHeld, Color.magenta);
            }
            
            if (InputManager.BlueightButtonWasPressed)
            {
                TryChangeColor(Color.blue, InputManager.RedLightButtonWasPressed, Color.magenta, InputManager.GreenLightButtonIsHeld, Color.cyan);
            }

            if (InputManager.GreenLightButtonWasPressed)
            {
                TryChangeColor(Color.green, InputManager.BlueLightButtonIsHeld, Color.cyan, InputManager.RedLightButtonWasPressed, Color.yellow);
            }
        }

        private void TryChangeColor(Color defaultColor, bool secondKey, Color colorIfBothPressed, bool thirdKey, Color colorIfThirdPressed)
        {
            Color newColor = defaultColor;
            if (secondKey && CanMixColors(defaultColor, colorIfBothPressed))
            {
                newColor = colorIfBothPressed;
            }
            else if (thirdKey && CanMixColors(defaultColor, colorIfThirdPressed))
            {
                newColor = colorIfThirdPressed;
            }
            _colorManager.ChangeColor(newColor);
        }

        private bool CanMixColors(Color color1, Color color2)
        {
            return (color1 == Color.red && color2 == Color.blue) ||
                   (color1 == Color.green && color2 == Color.blue) ||
                   (color1 == Color.red && color2 == Color.green);
        }
    }
}
