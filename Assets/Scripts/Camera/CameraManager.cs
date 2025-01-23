using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    public static CameraManager Instance;

    [SerializeField] private CinemachineVirtualCamera[] _allVirtualCameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")] [SerializeField]
    private float _fallPanAmount = 0.25f;

    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThreshold = -15f;

    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;
    [HideInInspector] public CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer _framingTransposer;
    private float _normYPanAmount;

    private Vector2 _startingTrackedObjectOffset;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if (_allVirtualCameras[i].enabled)
            {
                currentCamera = _allVirtualCameras[i];
                _framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        _normYPanAmount = _framingTransposer.m_YDamping;

        _startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;
    }

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = _framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }

        else
        {
            endDampAmount = _normYPanAmount;
        }

        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));
            _framingTransposer.m_YDamping = lerpedPanAmmount;
            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion

    #region PanCamera

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        if (!panToStartingPos)
        {
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.left;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;
            startingPos = _startingTrackedObjectOffset;
            endPos += startingPos;
        }


        else
        {
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
            _framingTransposer.m_TrackedObjectOffset = panLerp;
            yield return null;
        }
    }


    #endregion

    #region Swap Cameras

    public void SwapCameraRightLeft(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        if (currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            cameraFromRight.enabled = true;
            cameraFromLeft.enabled = false;
            currentCamera = cameraFromRight;
            _framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        else if (currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            cameraFromLeft.enabled = true;
            cameraFromRight.enabled = false;
            currentCamera = cameraFromLeft;
            _framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }
    
    public void SwapCameraTopBot(CinemachineVirtualCamera cameraFromTop, CinemachineVirtualCamera cameraFromBottom, Vector2 triggerExitDirection)
    {
        if (currentCamera == cameraFromTop && triggerExitDirection.y < 0f)
        {
            cameraFromBottom.enabled = true;
            cameraFromTop.enabled = false;
            currentCamera = cameraFromBottom;
            _framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        else if (currentCamera == cameraFromBottom && triggerExitDirection.y > 0f)
        {
            cameraFromTop.enabled = true;
            cameraFromBottom.enabled = false;
            currentCamera = cameraFromTop;
            _framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }



    #endregion
}
