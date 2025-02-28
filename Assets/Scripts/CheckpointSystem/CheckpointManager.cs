using System.Collections.Generic;
using UnityEngine;

namespace CheckpointSystem
{
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance;
        private List<Transform> _checkpointsList = new List<Transform>();
        private Transform _lastCheckpoint;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RegisterCheckpoint(Transform checkpoint)
        {
            if (!_checkpointsList.Contains(checkpoint))
                _checkpointsList.Add(checkpoint);
        }

        public void SetCheckpoint(Transform checkpoint)
        {
            _lastCheckpoint = checkpoint;
        }

        public Transform GetLastCheckpoint()
        {
            return _lastCheckpoint;
        }
    }
}