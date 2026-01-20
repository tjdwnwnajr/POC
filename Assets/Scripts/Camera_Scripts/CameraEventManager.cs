using Cinemachine;
using UnityEngine;

public class CameraEventManager : MonoBehaviour
{
    [Header("Camera Offset")]
    public CameraEventFocus cameraEventFocus;
    [SerializeField] private Transform offsetTarget;
    [SerializeField] private bool offsetOn;
    [Header("Camera Shake")]
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile profile;
    [SerializeField] private bool shakeOn;
    [Header("Mission Complete")]
    [SerializeField] private RotateBlock missionblock;
    [SerializeField] private DisappearBlock disappearBlock;
    private bool isComplete =false;
    private bool isDisappear = false;
    //종료 변수
    private bool isEnd = false;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();        
    }

    private void Update()
    {
        if (missionblock != null)
        {
            isComplete = missionblock.isComplete;
        }
        if (disappearBlock != null)
        {
            isDisappear = disappearBlock.isDisappear;
        }

        if (!isEnd)
        { 
            if (isComplete)
            {
                if (offsetOn)
                {
                    CameraOffsetEvent();
                }
                if (shakeOn)
                {
                    CameraShakeEvent();
                }
                isEnd = true;
            }
            
            if (isDisappear)
            {
                if (shakeOn)
                {
                    CameraShakeEvent();
                }
            }
            
        }
        if (disappearBlock != null)
        {
            isEnd = isDisappear;
            Debug.Log(isDisappear);
        }

    }
    private void CameraOffsetEvent()
    {
        if (cameraEventFocus != null)
        {
            PlayerStateList.isView = true;
            cameraEventFocus.FocusEvent(transform, offsetTarget);
            Invoke(nameof(ReturnCamera), 2f);
        }

    }
    private void ReturnCamera()
    {
        cameraEventFocus.ReturnToPlayer();
        PlayerStateList.ViewMode(false);
    }

    private void CameraShakeEvent()
    {
        CameraShakeManager.instance.ScreenShakeFromProfile(profile, impulseSource);

    }

    
}
