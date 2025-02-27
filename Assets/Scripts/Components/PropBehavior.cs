using System.Collections.Generic;
using Helper;
using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Components
{
    public class PropBehavior : MonoBehaviour
    {
        [SerializeField] private float _disabledOpacity = 0.5f;

        private List<SpriteRenderer> _spriteRendererList;
        private BoxCollider2D _boxCollider2D;
        private Light2D _playerLight;
        private float _startOpacity;

        private void Awake()
        {
            _spriteRendererList = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
            _boxCollider2D = GetComponent<BoxCollider2D>();
            if (_spriteRendererList.Count > 0)
            {
                _startOpacity = _spriteRendererList[0].color.a;
            }
        }

        private void Start()
        {
            _playerLight = PlayerMovement.Instance.GetComponentInChildren<Light2D>();
        }

        private void Update()
        {
            bool isMatching = false;

            foreach (var spriteRenderer in _spriteRendererList)
            {
                Color propColor = spriteRenderer.color;
                isMatching = ColorHelpers.Match(propColor, _playerLight.color);
                spriteRenderer.color = new Color(propColor.r, propColor.g, propColor.b, isMatching ? _startOpacity : _disabledOpacity);
            }
            
            _boxCollider2D.enabled = isMatching;
        }
    }
}