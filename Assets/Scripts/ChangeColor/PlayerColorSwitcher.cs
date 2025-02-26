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
            if (OldInputManager.instance.RedLightJustPressed)
            {
                ChangeColor(Color.red, OldInputManager.instance.GreenLightBeingHeld, Color.yellow, OldInputManager.instance.BlueLightBeingHeld, Color.magenta);
            }

            if (OldInputManager.instance.GreenLightJustPressed)
            {
                ChangeColor(Color.green, OldInputManager.instance.BlueLightBeingHeld, Color.cyan, OldInputManager.instance.RedLightBeingHeld, Color.yellow);
            }

            if (OldInputManager.instance.BlueLightJustPressed)
            {
                ChangeColor(Color.blue, OldInputManager.instance.RedLightBeingHeld, Color.magenta, OldInputManager.instance.GreenLightBeingHeld, Color.cyan);
            }
        }

        private void ChangeColor(Color defaultColor, bool secondKey, Color colorIfBothPressed, bool thirdKey, Color colorIfThirdPressed)
        {
            Color newColor = defaultColor;

            if (_colorManager.SwitchableColors.Count >= 2 && secondKey && CanMixColors(defaultColor, colorIfBothPressed))
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
