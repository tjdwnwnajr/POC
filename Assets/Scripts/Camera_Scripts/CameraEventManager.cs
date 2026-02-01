using Cinemachine;
using UnityEngine;

public class CameraEventManager : MonoBehaviour
{
    public static CameraEventManager instance;
    [Header("Camera Offset")]
    private CameraEventFocus cameraEventFocus;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        cameraEventFocus = GetComponent<CameraEventFocus>();
    }
    public void CameraOffsetEvent(Transform startPos, Transform targetPos)
    {
        if (cameraEventFocus != null)
        {
            PlayerStateList.isView = true;
            cameraEventFocus.FocusEvent(startPos, targetPos);
            Invoke(nameof(ReturnCamera), 2f);
        }

    }
    private void ReturnCamera()
    {
        cameraEventFocus.ReturnToPlayer();
        PlayerStateList.isView = false;
    }

    public void CameraShakeEvent(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        CameraShakeManager.instance.ScreenShakeFromProfile(profile, impulseSource);

    }


}
