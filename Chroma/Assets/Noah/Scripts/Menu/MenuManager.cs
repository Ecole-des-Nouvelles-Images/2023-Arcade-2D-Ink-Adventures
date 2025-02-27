using Noah.Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using PlayerController = Elias.Scripts.Components.PlayerController;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject _mainMenuCanvasGO;
    [SerializeField] private GameObject _settingsMenuCanvasGO;
    [SerializeField] private GameObject _volumeMenuCanvasGO;
    [SerializeField] private GameObject _keyboardMenuCanvasGO;
    [SerializeField] private GameObject _gamepadMenuCanvasGO;


    [Header("First Selected Options")] 
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;
    [SerializeField] private GameObject _volumeMenuFirst;
    [SerializeField] private GameObject _keyboardMenuFirst;
    [SerializeField] private GameObject _gamepadMenuFirst;


    private bool _isPaused;

    private void Start()
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _volumeMenuCanvasGO.SetActive(false);
        _keyboardMenuCanvasGO.SetActive(false);
        _gamepadMenuCanvasGO.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.instance.MenuOpenCloseInput)
        {
            if (!_isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
    }
    
    #region Pause/Unpause Functions
    private void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        PlayerController.Instance.enabled = false;
        OpenMainMenu();
    }

    private void Unpause()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        PlayerController.Instance.enabled = true;
        CloseAllMenus();
    }
    #endregion
    
    #region Canvas Activations

    private void OpenMainMenu()
    {
        _mainMenuCanvasGO.SetActive(true);
        _settingsMenuCanvasGO.SetActive(false);
        _volumeMenuCanvasGO.SetActive(false);
        _keyboardMenuCanvasGO.SetActive(false);
        _gamepadMenuCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }
    
    private void OpenSettingsMenuHandle()
    {
        _settingsMenuCanvasGO.SetActive(true);
        _mainMenuCanvasGO.SetActive(false);
        _volumeMenuCanvasGO.SetActive(false);
        _keyboardMenuCanvasGO.SetActive(false);
        _gamepadMenuCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_settingsMenuFirst);
    }
    
    private void OpenVolumeMenuHandle()
    {
        _volumeMenuCanvasGO.SetActive(true);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _gamepadMenuCanvasGO.SetActive(false);
        _keyboardMenuCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_volumeMenuFirst);
    }

    private void OpenKeyboardMenuHandle()
    {
        _keyboardMenuCanvasGO.SetActive(true);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _volumeMenuCanvasGO.SetActive(false);
        _gamepadMenuCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_keyboardMenuFirst);
    }
    private void OpenGamepadMenuHandle()
    {
        _gamepadMenuCanvasGO.SetActive(true);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _volumeMenuCanvasGO.SetActive(false);
        _keyboardMenuCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_gamepadMenuFirst);
    }


    private void CloseAllMenus()
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _volumeMenuCanvasGO.SetActive(false);
        _keyboardMenuCanvasGO.SetActive(false);
        _gamepadMenuCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    #endregion

    #region Main Menu Button Actions

    public void OnSettingsPress()
    {
        OpenSettingsMenuHandle();
    }

    public void OnResumePress()
    {
        Unpause();
    }
    #endregion

    #region Settings Menu Button Actions

    public void OnSettingsVolumePress()
    {
        OpenVolumeMenuHandle();
    }
    public void OnSettingsKeyboardPress()
    {
        OpenKeyboardMenuHandle();
    }
    public void OnSettingsGamepadPress()
    {
        OpenGamepadMenuHandle();
    }
    public void OnSettingsBackPress()
    {
        OpenMainMenu();
    }
    
    
    #endregion
    
}
