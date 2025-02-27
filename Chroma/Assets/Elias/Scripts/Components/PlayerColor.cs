using System.Collections.Generic;
using Elias.Scripts.Helper;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Unity.UI;
using UnityEngine.UI;

namespace Elias.Scripts.Components
{
    public class PlayerColor : MonoBehaviour {
        
        public List<Color> switchableColors = new List<Color>();

        [SerializeField]
        private Image _UIBulb;

        private Light2D _playerLight;
        private List<PropBehavior> _propColorColliders = new List<PropBehavior>();
        

        private void Awake() {
            _playerLight = GetComponent<Light2D>();
        }

        private void Update() {
            InputSwitchColor();
            _UIBulb.color = _playerLight.color;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PropBehavior propBehavior = other.GetComponent<PropBehavior>();
            if (other.CompareTag("Upgrader"))
            {
                switchableColors.Add(other.gameObject.GetComponent<Light2D>().color);
                Destroy(other.gameObject); 
            }
            else if (propBehavior)
            {
                _propColorColliders.Add(propBehavior);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            PropBehavior propBehavior = other.GetComponent<PropBehavior>();
            if (propBehavior)
            {
                _propColorColliders.Remove(propBehavior);
            }
        }
        
        private void InputSwitchColor()
        {
            if (InputManager.instance.RedLightJustPressed) {
                ChangeColor(InputManager.instance.GreenLightBeingHeld, 
                        Color.yellow, InputManager.instance.BlueLightBeingHeld, 
                        Color.magenta, Color.red);
                
            }

            if (InputManager.instance.GreenLightJustPressed) {
                ChangeColor(InputManager.instance.BlueLightBeingHeld, 
                    Color.cyan, InputManager.instance.RedLightBeingHeld, 
                    Color.yellow, Color.green);
                
            }
            
            if (InputManager.instance.BlueLightJustPressed) {
                ChangeColor(InputManager.instance.RedLightBeingHeld, 
                    Color.magenta, InputManager.instance.GreenLightBeingHeld, 
                    Color.cyan, Color.blue);
                
            }
        }

        private void ChangeColor(bool secondKey, Color colorIfBothPressed, bool thirdKey, Color colorIfThirdPressed, Color defaultColor)
        {
            if (!switchableColors.Contains(defaultColor)) return;
            Color color = defaultColor;

            if (secondKey)
            {
                if (colorIfBothPressed == Color.magenta && switchableColors.Contains(Color.red) && switchableColors.Contains(Color.blue))
                {
                    color = colorIfBothPressed;
                }
                else if (colorIfBothPressed == Color.cyan && switchableColors.Contains(Color.green) && switchableColors.Contains(Color.blue))
                {
                    color = colorIfBothPressed;
                }
                else if (colorIfBothPressed == Color.yellow && switchableColors.Contains(Color.red) && switchableColors.Contains(Color.green))
                {
                    color = colorIfBothPressed;
                }
            }
            else if (thirdKey)
            {
                if (colorIfThirdPressed == Color.magenta && switchableColors.Contains(Color.red) && switchableColors.Contains(Color.blue))
                {
                    color = colorIfThirdPressed;
                }
                else if (colorIfThirdPressed == Color.cyan && switchableColors.Contains(Color.green) && switchableColors.Contains(Color.blue))
                {
                    color = colorIfThirdPressed;
                }
                else if (colorIfThirdPressed == Color.yellow && switchableColors.Contains(Color.red) && switchableColors.Contains(Color.green))
                {
                    color = colorIfThirdPressed;
                }
            }


            foreach (PropBehavior propColorCollider in _propColorColliders)
            {
                SpriteRenderer propSpriteRenderer = propColorCollider.GetComponent<SpriteRenderer>();
                if (ColorHelpers.Match(propSpriteRenderer.color, color)) return;
            }
            _playerLight.color = color;
            PlayerController.Instance.PlayRandomLampSound();
        }
    }
}