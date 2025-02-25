using System.Collections.Generic;
using Components;
using Elias.Scripts.Helper;
using Helper;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ChangeColor
{
    public class PropColorHandler : MonoBehaviour
    {
        private List<PropBehavior> _propColorColliders = new List<PropBehavior>();
        private PlayerColorManager _colorManager;

        private void Start()
        {
            _colorManager = GetComponent<PlayerColorManager>();
            _colorManager.OnColorChanged += OnPlayerColorChanged;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PropBehavior propBehavior = other.GetComponent<PropBehavior>();
            if (other.CompareTag("Upgrader"))
            {
                _colorManager.SwitchableColors.Add(other.gameObject.GetComponent<Light2D>().color);
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

        private void OnPlayerColorChanged(Color newColor)
        {
            foreach (PropBehavior propColorCollider in _propColorColliders)
            {
                SpriteRenderer propSpriteRenderer = propColorCollider.GetComponent<SpriteRenderer>();
                if (ColorHelpers.Match(propSpriteRenderer.color, newColor)) return;
            }
        }
    }
}