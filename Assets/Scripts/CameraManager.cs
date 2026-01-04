using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;


public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    [SerializeField] private CinemachineVirtualCamera[] _allVirtualCameras;

    [Header("lerping Y Damping")]
    //낙하 y반응속도
    [SerializeField] private float _fallPanAmount = 0.25f;
    //y반응속도 변화 시간
    [SerializeField] private float _fallYPanTime = 0.35f;
    //낙하 속도 기준값
    public float _fallSpeedYDampingChangeThresholde = -15f;

    //현재 카메라 값 조정중인지 여부
    public bool IsLerpingYDamping { get; private set; }

    //낙하로 인해 카메라 값 조정이 되었는지 여부
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    //실제 카메라 y반응속도 조절 컴포넌트
    private CinemachineFramingTransposer _framingTransposer;
    //현재 활성 가상카메라
    private CinemachineVirtualCamera _currentCamera;
    //평상시 y반응속도 값
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
    //플레이어 낙하중인지 여부에 따라 y반응속도를 조절하도록 외부에서 호출하는 함수
    public void LerpYDamping(bool isPlayerFalling)
    {
        //이전 코루틴이 실행중이면 null이 아니어서 중지한다.
        if(_lerpYPanCoroutine != null)
        {
            StopCoroutine(_lerpYPanCoroutine);
        }

        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling)); 
    }

    //실제 y반응속도 조절 코루틴
    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        //조정중임을 true로 설정
        IsLerpingYDamping = true;

        //낙하 시작값 설정
        float startDampAmount = _framingTransposer.m_YDamping;

        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            //조정되었음을 알리도록 true로 설정
            LerpedFromPlayerFalling = true;
        }
        else
        {
            //조정안되는 상황이므로 원래 y반응속도 값으로 복귀
            endDampAmount = _normYPanAmount;
        }

        float elapsedTime = 0f;
       
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;
            
            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));

            _framingTransposer.m_YDamping = lerpedPanAmount;
            //다음프레임까지 대기
            yield return null;
        }
        //조정 종료
        IsLerpingYDamping = false;
    }
}
