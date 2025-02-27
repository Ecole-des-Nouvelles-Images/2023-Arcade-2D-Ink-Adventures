using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu Objects")]
    [SerializeField] private GameObject _loadingBarObject;
    [SerializeField] private Slider _sliderBar;
    [SerializeField] private GameObject[] _objectsToHide;
    
    [Header("First Selected Options")] 
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;
    [SerializeField] private GameObject _volumeMenuFirst;
    [SerializeField] private GameObject _keyboardMenuFirst;
    [SerializeField] private GameObject _gamepadMenuFirst;
    [SerializeField] private GameObject _creditsMenuFirst;

    
    [Header("Scenes to load")] 
    [SerializeField] private SceneField _persistentGameplay;
    [SerializeField] private SceneField _levelScene;
    
    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();
    private void Awake()
    { 
        _loadingBarObject.SetActive(false);
        OnFirstSelectedMainButton();
    }
    
    public void OnFirstSelectedMainButton()
    {
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }
    public void OnFirstSelectedSettingsButton()
    {
        EventSystem.current.SetSelectedGameObject(_settingsMenuFirst);
    }
    public void OnFirstSelectedVolumeButton()
    {
        EventSystem.current.SetSelectedGameObject(_volumeMenuFirst);
    }
    public void OnFirstSelectedKeyboardButton()
    {
        EventSystem.current.SetSelectedGameObject(_keyboardMenuFirst);
    }
    public void OnFirstSelectedGamepadButton()
    {
        EventSystem.current.SetSelectedGameObject(_gamepadMenuFirst);
    }
    public void OnFirstSelectedCreditsButton()
    {
        EventSystem.current.SetSelectedGameObject(_creditsMenuFirst);
    }

    public void StartGame()
    {
        HideMenu();
        _loadingBarObject.SetActive(true); 
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_persistentGameplay));
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_levelScene, LoadSceneMode.Additive));
        StartCoroutine(ProgressLoadingBar()); 
    }

    public void QuitGame()
    {
        QuitGame();
    }

    private void HideMenu()
    {
        for (int i = 0; i < _objectsToHide.Length; i++)
        {
            _objectsToHide[i].SetActive(false);
        }
    }

    private IEnumerator ProgressLoadingBar()
    {
        float loadProgress = 0f;
        for (int i = 0; i < _scenesToLoad.Count; i++)
        {
            while (!_scenesToLoad[i].isDone)
            {
                loadProgress += _scenesToLoad[i].progress;
                _sliderBar.value = loadProgress / _scenesToLoad.Count;
                yield return null;
            }
        }
    }
}
