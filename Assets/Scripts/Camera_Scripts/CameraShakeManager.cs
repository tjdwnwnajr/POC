using UnityEngine;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager instance;
    [SerializeField] private float globalShakeForce = 1f;
    private CinemachineImpulseListener impulseListener;
    private CinemachineVirtualCamera currentVirtualCamera;

    private CinemachineImpulseDefinition impulseDefinition;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //카메라찾기
        FindAndSetupVirtualCamera();
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }
    public void ScreenShakeFromProfile(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        SetupScreenShakeSettings(profile, impulseSource);

        impulseSource.GenerateImpulseWithForce(profile.impactForce);

    }
    private void SetupScreenShakeSettings(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        impulseDefinition = impulseSource.m_ImpulseDefinition;

        //change the impulse source settings
        impulseDefinition.m_ImpulseDuration = profile.impactTime;
        impulseSource.m_DefaultVelocity = profile.defaultVelocity;
        if (profile.useCustomCurve)
        {
            impulseDefinition.m_CustomImpulseShape = profile.impulseCurve;
        }
        else
        {
            impulseDefinition.m_ImpulseShape = profile.shapes;
        }
        

        //change the impulse listener settings
        impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplistude;
        impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
        impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;

    }
    public void FindAndSetupVirtualCamera()
    {
        currentVirtualCamera = CameraUtility.GetActiveVirtualCamera();
        
        if(currentVirtualCamera == null)
        {
            Debug.LogError("CameraShakeManager에서 카메라를 찾지 못하였습니다.");
            return;
        }

        // 그 카메라에서 Listener 추출
        impulseListener = currentVirtualCamera.GetComponent<CinemachineImpulseListener>();
    }

}
