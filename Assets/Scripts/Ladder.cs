using System;
using System.Collections.Generic;
using Common;
using Components;
using Player;
using UnityEngine;
public class Ladder : MonoBehaviour
{
    private List<BoxCollider2D> _boxColliders2DList;
    [SerializeField] private bool _lockPlayerToLadder; 

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement.Instance._isClimbing = true;
            PlayerMovement.Instance._isOnLadder = true;

            if (_lockPlayerToLadder)
            {
                GameEvents.OnPlayerClimb.Invoke(this.gameObject.transform);
            }

        }        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement.Instance._isClimbing = false;
            PlayerMovement.Instance._isOnLadder = false;
            
            if (_lockPlayerToLadder)
            {
                GameEvents.OnPlayerStopClimb.Invoke(this.gameObject.transform);
            }

        }  
    }
}
