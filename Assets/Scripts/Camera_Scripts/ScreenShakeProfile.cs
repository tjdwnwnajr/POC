using Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "ScreenShakeProfile", menuName = "Scriptable Objects/ScreenShakeProfile")]
public class ScreenShakeProfile : ScriptableObject
{
    [Header("Impulse Source Settings")]
    //카메라가 얼마나 오래 흔들릴지 
    public float impactTime = 0.5f;
    //흔들림 강도
    public float impactForce = 1f;
    //흔들림 초기 방향
    public Vector3 defaultVelocity = new Vector3(0f, -1f, 0f);
    //시간에 따른 흔들림 강도 변화 곡선
    public AnimationCurve impulseCurve;
    //기본 곡선
    public bool useCustomCurve;
    //곡선 선택
    public CinemachineImpulseDefinition.ImpulseShapes shapes;
    [Header("Impulse listener Settings")]
    //카메라가 얼마나 크게 움직일지
    public float listenerAmplistude = 1f;
    //카메라 흔들림 반복수
    public float listenerFrequency = 1f;
    //카메라 흔들림 지속시간
    public float listenerDuration = 1f;
    
    


}
