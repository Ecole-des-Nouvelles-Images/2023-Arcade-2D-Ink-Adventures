using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Ending : MonoBehaviour
{
    [SerializeField] private SceneField _levelScene;
    public float sceneChangeDelay = 50f;
    private float _timer;

    private void Start()
    {
        _timer = 0f;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= sceneChangeDelay)
        {
            ChangeScene();
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadSceneAsync(_levelScene);
    }
}

