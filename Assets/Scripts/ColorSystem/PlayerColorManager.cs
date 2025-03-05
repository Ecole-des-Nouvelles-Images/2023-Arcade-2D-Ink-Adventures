using System;
using System.Collections.Generic;
using Common;
using Components;
using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ChangeColor
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
            if (!SwitchableColors.Contains(newColor)) return;
            Debug.Log("test");
            _playerLight.color = newColor;
            GameEvents.OnColorChanged?.Invoke(newColor);
        }
    }
}