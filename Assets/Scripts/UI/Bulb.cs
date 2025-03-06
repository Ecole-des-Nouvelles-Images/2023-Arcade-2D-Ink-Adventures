using System;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Bulb : MonoBehaviour
    {
        private Image _bulbImage;
        private void OnEnable()
        {
            GameEvents.OnColorChanged += ChangeBulbColor;
        }
        private void OnDisable()
        {
            GameEvents.OnColorChanged -= ChangeBulbColor;
        }

        private void Start()
        {
            _bulbImage = GetComponent<Image>();
        }

        private void ChangeBulbColor(Color color)
        {
            _bulbImage.color = color;
        }
    }
}
