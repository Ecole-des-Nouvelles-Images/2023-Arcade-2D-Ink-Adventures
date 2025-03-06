using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ColorSystem
{
    public class PlayerColorManager : MonoBehaviour
    {
        public List<Color> SwitchableColors = new List<Color>();
        private Light2D _playerLight;

        private void Start()
        {
            _playerLight = GetComponentInChildren<Light2D>();
        }

        public void ChangeColor(Color newColor)
        {
            if (_playerLight == null) return;

            if (SwitchableColors.Contains(newColor) || CanMixColor(newColor))
            {
                _playerLight.color = newColor;
                GameEvents.OnColorChanged?.Invoke(newColor);
            }
        }

        private bool CanMixColor(Color color)
        {
            var mixableColors = new Dictionary<Color, (Color, Color)>
            {
                { Color.magenta, (Color.red, Color.blue) },
                { Color.yellow, (Color.red, Color.green) },
                { Color.cyan, (Color.blue, Color.green) }
            };

            if (mixableColors.TryGetValue(color, out var baseColors))
            {
                return SwitchableColors.Contains(baseColors.Item1) && SwitchableColors.Contains(baseColors.Item2);
            }

            return false;
        }
    }
}