using Input;
using UnityEngine;

namespace ColorSystem
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
            if (InputManager.RedLightButtonWasPressed && !InputManager.BlueLightButtonIsHeld && !InputManager.GreenLightButtonIsHeld)
            {
                _colorManager.ChangeColor(Color.red);
            }
            
            if (InputManager.BlueLightButtonWasPressed && !InputManager.RedLightButtonIsHeld && !InputManager.GreenLightButtonIsHeld)
            {
                _colorManager.ChangeColor(Color.blue);
            }
            
            if (InputManager.GreenLightButtonWasPressed)
            {
                _colorManager.ChangeColor(Color.green);
            }

            if ((InputManager.RedLightButtonWasPressed || InputManager.RedLightButtonIsHeld) && (InputManager.BlueLightButtonWasPressed || InputManager.BlueLightButtonIsHeld))
            {
                _colorManager.ChangeColor(Color.magenta);
            }
            
            if ((InputManager.RedLightButtonWasPressed || InputManager.RedLightButtonIsHeld) && (InputManager.GreenLightButtonIsHeld || InputManager.GreenLightButtonIsHeld))
            {
                _colorManager.ChangeColor(Color.yellow);
            }            
            
            if ((InputManager.BlueLightButtonWasPressed || InputManager.BlueLightButtonIsHeld) && (InputManager.GreenLightButtonIsHeld || InputManager.GreenLightButtonIsHeld))
            {
                _colorManager.ChangeColor(Color.cyan);
            }
        }
    }
}
