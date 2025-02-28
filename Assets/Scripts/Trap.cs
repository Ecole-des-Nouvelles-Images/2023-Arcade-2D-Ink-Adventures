using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trap");
            GameEvents.OnPlayerDeath?.Invoke();
        }
    }
}
