using System.Collections.Generic;
using Helper;
using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum StartColor
{
    Red,
    Blue,
    Green,
    Magenta,
    Yellow,
    Cyan
}

namespace Components
{
    public class PropBehavior : MonoBehaviour
    {
        [SerializeField] private StartColor _startColor;
        [SerializeField] private float _disabledOpacity = 0.5f;
        
        private List<SpriteRenderer> _spriteRenderersList;
        private List<BoxCollider2D> _boxColliders2DList;
        private Light2D _playerLight;

        private void Awake()
        {
            _spriteRenderersList = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
            _boxColliders2DList = new List<BoxCollider2D>(GetComponentsInChildren<BoxCollider2D>());

            if (_spriteRenderersList.Count > 0)
            {
                Color initialColor = GetColorFromEnum(_startColor);
                foreach (var spriteRenderer in _spriteRenderersList)
                {
                    spriteRenderer.color = initialColor;
                }
            }
        }

        private void Start()
        {
            _playerLight = PlayerMovement.Instance.GetComponentInChildren<Light2D>();
        }

        private void Update()
        {
            if (_playerLight == null) return;

            bool isMatching = false;
            
            foreach (SpriteRenderer spriteRenderer in _spriteRenderersList)
            {
                Color propColor = spriteRenderer.color;
                bool match = ColorHelpers.Match(propColor, _playerLight.color);

                if (match)
                {
                    isMatching = true;
                }

                spriteRenderer.color = new Color(propColor.r, propColor.g, propColor.b, match ? 1f : _disabledOpacity);
            }

            foreach (BoxCollider2D boxCollider2D in _boxColliders2DList)
            {
                boxCollider2D.enabled = isMatching;
            }
        }
        
        private Color GetColorFromEnum(StartColor color)
        {
            return color switch
            {
                StartColor.Red => Color.red,
                StartColor.Blue => Color.blue,
                StartColor.Green => Color.green,
                StartColor.Magenta => Color.magenta,
                StartColor.Yellow => Color.yellow,
                StartColor.Cyan => Color.cyan,
                _ => Color.white
            };
        }
    }
}
