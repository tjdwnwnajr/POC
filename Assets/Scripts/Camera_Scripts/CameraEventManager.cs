using Cinemachine;
using System.Collections;
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
    public void CameraOffsetEvent(Transform startPos, Transform targetPos, float time = 2f, bool returnNow = false, float moveduration = 0.15f)
    {
        PlayerStateList.isView = true;
        CameraEventFocus.instance.FocusEvent(startPos, targetPos,moveduration);

        
        if (returnNow)
        {
            ReturnCamera();
        }
        else
        {
            StartCoroutine(WaitForReturn(time));
        }

    }

    IEnumerator WaitForReturn(float time)
    {
        yield return new WaitForSeconds(time);
        ReturnCamera();
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
