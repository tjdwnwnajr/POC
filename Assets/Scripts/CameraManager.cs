using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;


public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    [SerializeField] private CinemachineVirtualCamera[] _allVirtualCameras;

    [Header("lerping Y Damping")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThresholde = -15f;

    public bool IsLerpingYDamping { get; private set; }

    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;

    private CinemachineFramingTransposer _framingTransposer;
    private CinemachineVirtualCamera _currentCamera;

    private float _normYPanAmount;

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
                _currentCamera = _allVirtualCameras[i];
                
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                _normYPanAmount = _framingTransposer.m_YDamping;
            }
        }
    } 

    public void LerpYDamping(bool isPlayerFalling)
    {
        if(_lerpYPanCoroutine != null)
        {
            StopCoroutine(_lerpYPanCoroutine);
        }
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
            
            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));
            _framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }
        IsLerpingYDamping = false;
    }
}
