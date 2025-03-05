using Input;
using UnityEngine;

namespace ChangeColor
{
    public class PlayerColorSwitcher : MonoBehaviour
    {
        private PlayerColorManager _colorManager;

        private void Start()
        {
            _colorManager = GetComponent<PlayerColorManager>();
        }


        private void Update()
        {
            HandleColorSwitchInput();
        }

        private void HandleColorSwitchInput()
        {
            if (InputManager.RedLightButtonWasPressed)
            {
                Debug.Log("Red light");
                SubmitColorChange(Color.red,InputManager.GreenLightButtonIsHeld, Color.yellow, InputManager.BlueLightButtonIsHeld, Color.magenta);
            }
            
            if (InputManager.BlueLightButtonWasPressed)
            {
                Debug.Log("Blue light");
                SubmitColorChange(Color.blue, InputManager.RedLightButtonIsHeld, Color.magenta, InputManager.GreenLightButtonIsHeld, Color.cyan);
            }

            if (InputManager.GreenLightButtonWasPressed)
            {
                Debug.Log("Green light");
                SubmitColorChange(Color.green, InputManager.BlueLightButtonIsHeld, Color.cyan, InputManager.RedLightButtonIsHeld, Color.yellow);
            }
        }

        private void SubmitColorChange(Color defaultColor, bool secondKey, Color colorIfBothPressed, bool thirdKey, Color colorIfThirdPressed)
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
