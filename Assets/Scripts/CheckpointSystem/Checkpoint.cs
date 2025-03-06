    using UnityEngine;

namespace CheckpointSystem
{
    public class Checkpoint : MonoBehaviour
    {
        private void Start()
        {
            CheckpointManager.Instance.RegisterCheckpoint(transform);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                CheckpointManager.Instance.SetCheckpoint(transform);
            }
        }
    }
}