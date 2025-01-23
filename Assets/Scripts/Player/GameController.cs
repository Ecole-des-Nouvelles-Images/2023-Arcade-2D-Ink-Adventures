using System;
using System.Collections;
using UnityEngine;

namespace Noah.Scripts.Player
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        [SerializeField] private float respawnTime = 0.1f;
        private Vector2 _checkpointPos;
        private Rigidbody2D _playerRb;

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            _checkpointPos = transform.position;
        }
    
        public void UpdateCheckpoint(Vector2 pos)
        {
            _checkpointPos = pos;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Obstacle"))
            {
                Die();
            }
        }

        private void Die()
        {
            StartCoroutine(Respawn(respawnTime));
        }

        IEnumerator Respawn(float duration)
        { 
            transform.position = _checkpointPos; 
            yield return new WaitForSeconds(duration);
        }
    
    }
}
