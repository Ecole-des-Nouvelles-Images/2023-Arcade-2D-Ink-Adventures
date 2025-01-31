using System.Collections.Generic;
using Components;
using Elias.Scripts.Helper;
using Input;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Elias.Scripts.Components
{
    public class PlayerColor : MonoBehaviour {
        
        public List<Color> switchableColors = new List<Color>();
        [SerializeField]
        private Image _UIBulb;

        private Light2D _playerLight;
        private List<PropBehavior> _propColorColliders = new List<PropBehavior>();

        

        private void Start()
        {
            _playerLight = PlayerController.Instance.GetComponentInChildren<Light2D>();
        }
        

        private void Update() {
            InputSwitchColor();
            if (_UIBulb != null)
            {
                _UIBulb.color = _playerLight.color;
            }
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
            if (OldInputManager.instance.RedLightJustPressed) {
                ChangeColor(OldInputManager.instance.GreenLightBeingHeld, 
                        Color.yellow, OldInputManager.instance.BlueLightBeingHeld, 
                        Color.magenta, Color.red);
                
            }

            if (OldInputManager.instance.GreenLightJustPressed) {
                ChangeColor(OldInputManager.instance.BlueLightBeingHeld, 
                    Color.cyan, OldInputManager.instance.RedLightBeingHeld, 
                    Color.yellow, Color.green);
                
            }
            
            if (OldInputManager.instance.BlueLightJustPressed) {
                ChangeColor(OldInputManager.instance.RedLightBeingHeld, 
                    Color.magenta, OldInputManager.instance.GreenLightBeingHeld, 
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