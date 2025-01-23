using System;
using System.Collections;
using System.Collections.Generic;
using Noah.Scripts.Player;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine;
using PlayerController = Elias.Scripts.Components.PlayerController;

public class CustomizeMenuManager : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject _customizeMenuGO;
    
    [Header("First Selected Options")] 
    [SerializeField] private GameObject _customizeMenuFirst;

    private bool _onCustomizing;

    private bool _onPause;

    private void Start()
    {
        Pause();
        OpenMenu();
        _onPause = false;
    }
    private void Update()
    {
        if (InputManager.instance.MenuOpenCloseInput && !_onPause)
        {
            _onPause = true;
        }
        if (InputManager.instance.MenuOpenCloseInput && _onPause)
        {
            _onPause = false;
        }
        if (_onCustomizing)
        {
            PlayerController.Instance._anim.SetBool("IsWalking", false);
            PlayerController.Instance.canMove = false;
            PlayerController.Instance.canJump = false;
            PlayerController.Instance.audioSource.mute = true;
        }
        else
        {
            PlayerController.Instance.canMove = true;
            PlayerController.Instance.audioSource.mute = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Pause();
            OpenMenu();
        }  
    }

    public void OnCustomValidate()
    {
        Unpause();
        CloseMenu();
    }
    
    private void Pause()
    {
        _onCustomizing = true;
        Time.timeScale = 1f;
        PlayerController.Instance.canJump = false;
        PlayerController.Instance.canMove = false;
    }

    private void Unpause()
    {
        StartCoroutine(EnableJumpAfterDelay());
        _onCustomizing = false;
        Time.timeScale = 1f;
        PlayerController.Instance.canMove = true;
    }

    private void OpenMenu()
    {
        _customizeMenuGO.SetActive(true);        
        EventSystem.current.SetSelectedGameObject(_customizeMenuFirst);
    }

    private void CloseMenu()
    {
        _customizeMenuGO.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator EnableJumpAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerController.Instance.canJump = true;
    }
}
