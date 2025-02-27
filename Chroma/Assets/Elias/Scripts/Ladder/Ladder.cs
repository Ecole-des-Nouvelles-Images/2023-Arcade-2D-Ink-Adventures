using UnityEngine;
using PlayerController = Elias.Scripts.Components.PlayerController;

namespace Elias.Scripts.Ladder
{
    public class Ladder : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                 PlayerController.IsClimbing = true;
            }        
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController.IsClimbing = false;
            }  
        }
    }
}
