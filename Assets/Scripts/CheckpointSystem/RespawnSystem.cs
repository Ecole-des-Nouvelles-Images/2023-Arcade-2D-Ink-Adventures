using System;
using Common;
using Player;
using UnityEngine;

namespace CheckpointSystem
{
    public class RespawnSystem : MonoBehaviour
    {
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
            Debug.Log(respawnCheckpoint.position);
            PlayerMovement.Instance.transform.position = respawnCheckpoint.position;
        }
    }
}
