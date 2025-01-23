using System;
using Noah.Scripts.Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace Noah.Scripts.Checkpoint
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private GameObject _lightHolder;
        private Light2D _light2D;
        private Collider2D _coll;

        private void Awake()
        {
            _coll = GetComponent<Collider2D>();
        }

        private void Start()
        {
            _light2D = _lightHolder.GetComponent<Light2D>();
            _light2D.color = Color.black;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameController.instance.UpdateCheckpoint(respawnPoint.position);
                _light2D.color = Color.white;
                _coll.enabled = false;
            }
        }
    }
    
}
