using System;
using Common;
using Player;
using UnityEngine;

namespace CheckpointSystem
{
    public class RespawnSystem : MonoBehaviour
    {
        [SerializeField] private float _respawnTime = 3f;

        private void OnEnable()
        {
            GameEvents.OnPlayerDeath += RespawnPlayer;
        }
        private void OnDisable()
        {
            GameEvents.OnPlayerDeath -= RespawnPlayer;
        }

        void RespawnPlayer()
        {
            Transform respawnCheckpoint = CheckpointManager.Instance.GetLastCheckpoint();
            PlayerMovement.Instance.transform.position = respawnCheckpoint.position;
        }
    }
}
