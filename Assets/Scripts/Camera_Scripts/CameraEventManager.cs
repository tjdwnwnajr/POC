using Cinemachine;
using UnityEngine;

public class CameraEventManager : MonoBehaviour
{
    public static CameraEventManager instance;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        
    }
    public void CameraOffsetEvent(Transform startPos, Transform targetPos)
    {
        PlayerStateList.isView = true;
        CameraEventFocus.instance.FocusEvent(startPos, targetPos);
        Invoke(nameof(ReturnCamera), 2f);
       

    }
    private void ReturnCamera()
    {
        CameraEventFocus.instance.ReturnToPlayer();
        PlayerStateList.isView = false;
    }

    public void CameraShakeEvent(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        CameraShakeManager.instance.ScreenShakeFromProfile(profile, impulseSource);

    }


}
