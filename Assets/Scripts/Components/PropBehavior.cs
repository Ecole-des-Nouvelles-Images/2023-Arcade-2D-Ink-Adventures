using System;
using Elias.Scripts.Helper;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Elias.Scripts.Components
{
    public class PropBehavior : MonoBehaviour
    {
        
        [Serializable]
        enum PropBehaviorType
        {
            Collider,
            Interactable,
            Danger
        }

        [SerializeField] private PropBehaviorType propBehaviorType;
        [SerializeField] private float disabledOpacity;

        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider2D;
        private GameObject _playerGameObject;
        private Light2D _playerLight;
        private float _startOpacity;
        private string _originalTag;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _startOpacity = _spriteRenderer.color.a;
        }

        private void Start()
        {
            _originalTag = gameObject.tag;
            _playerGameObject = GameObject.FindGameObjectWithTag("Player");
            _playerLight = _playerGameObject.GetComponent<Light2D>();
        }

        private void Update()
        {
            Color propColor = _spriteRenderer.color;
            bool isMatching = ColorHelpers.Match(propColor, _playerLight.color);
            gameObject.tag = !isMatching ? "Untagged" : _originalTag;

            switch (propBehaviorType)
            {
                case PropBehaviorType.Collider:
                    _spriteRenderer.color = new Color(propColor.r, propColor.g, propColor.b, isMatching ? _startOpacity : disabledOpacity);
                    _boxCollider2D.isTrigger = !isMatching;
                    break;
                case PropBehaviorType.Interactable:
                    _spriteRenderer.color = new Color(propColor.r, propColor.g, propColor.b, isMatching ? _startOpacity : disabledOpacity);
                    _boxCollider2D.enabled = isMatching;
                    break;
                case PropBehaviorType.Danger:
                    _spriteRenderer.color = new Color(propColor.r, propColor.g, propColor.b, isMatching ? _startOpacity : disabledOpacity);
                    _boxCollider2D.enabled = isMatching;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}