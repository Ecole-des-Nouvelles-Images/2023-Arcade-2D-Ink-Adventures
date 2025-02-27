using UnityEngine;
using UnityEngine.Serialization;

namespace Elias.Scripts.Components
{
    public class TriggerForestSound : MonoBehaviour
    {
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController.Instance.isInCity = false;
            }
        }
    }
}
