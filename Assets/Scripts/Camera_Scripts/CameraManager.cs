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


    #region Camera panning in camera control triggers
    private Coroutine _panCameraCoroutine;
    private Vector2 _startingTrackedObjectOffset;

    #endregion

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


        _startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;
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

    #region Pan Camera
    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }
    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        //handle pan from trigger
        if (!panToStartingPos)
        {
            //set the direction and distance
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;
                default:
                    endPos = _startingTrackedObjectOffset;
                    break;
            }
            endPos *= panDistance;
            startingPos = _startingTrackedObjectOffset;
            endPos += startingPos;

        }
        //handle the pan back to starting position
        else
        {
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }
        float elapesedTime = 0f;
        while (elapesedTime < panTime)
        {
            elapesedTime += Time.deltaTime;
            Vector2 lerpedPos = Vector2.Lerp(startingPos, endPos, (elapesedTime / panTime));
            _framingTransposer.m_TrackedObjectOffset = lerpedPos;
            yield return null;
        }

    }
    #endregion

    #region Swap Camera

    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        if (_currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            //if the curent camera is the camera on the left and our trigger exit direction was on the right
            cameraFromRight.enabled = true;
            //decativate the old camera
            cameraFromLeft.enabled = false;
            //set the new camera as the current camera
            _currentCamera = cameraFromRight;

            //update our composer variable
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        //if the current camera is the camera on the right and our trigger exit direction was on the left
        else if (_currentCamera ==cameraFromRight&&triggerExitDirection.x<0f) {
            cameraFromLeft.enabled = true;
            //decativate the old camera
            cameraFromRight.enabled = false;
            //set the new camera as the current camera
            _currentCamera = cameraFromLeft;

            //update our composer variable
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        CameraEventFocus.instance.RefreshCamera();
        CameraShakeManager.instance.RefreshCamera();
    }
    #endregion
}
