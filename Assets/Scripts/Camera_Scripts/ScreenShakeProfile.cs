using UnityEngine;

[CreateAssetMenu(fileName = "ScreenShakeProfile", menuName = "Scriptable Objects/ScreenShakeProfile")]
public class ScreenShakeProfile : ScriptableObject
{
    [Header("Impulse Source Settings")]
    public float impactTime = 0.5f;
    public float impactForce = 1f;
    public Vector3 defaultVelocity = new Vector3(0f, -1f, 0f);
    public AnimationCurve impulseCurve;

    [Header("Impulse listener Settings")]
    public float listenerAmplistude = 1f;
    public float listenerFrequency = 1f;
    public float listenerDuration = 1f;



}
