using UnityEngine;

public class MapRotation : MonoBehaviour
{
    public Transform mapParent;
    public Transform centerObject;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float rotationProgress = 0f;
    private float rotationDuration = 1f; // 1초
    private float shakeStartDuration = 0.5f; // 흔들림 0.5초
    private float totalDuration = 1.5f; // 전체 1.5초 (0.5초 흔들림 + 1초 회전)
    private bool isRotating = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isRotating)
        {
            StartRotation(90f);
        }

        if (isRotating)
        {
            rotationProgress += Time.deltaTime / totalDuration;

            if (rotationProgress >= 1f)
            {
                rotationProgress = 1f;
                isRotating = false;
            }

            if (rotationProgress < shakeStartDuration / totalDuration)
            {
                // 처음 0.5초: 좌우로 흔들림
                float shakeProgress = rotationProgress / (shakeStartDuration / totalDuration);
                float shake = Mathf.Sin(shakeProgress * Mathf.PI * 4f) * 0.5f; // 좌우 5도씩
                mapParent.rotation = startRotation * Quaternion.AngleAxis(shake, Vector3.forward);
            }
            else
            {
                // 0.5초 후: 정상 회전
                float actualProgress = (rotationProgress - shakeStartDuration / totalDuration) / (rotationDuration / totalDuration);
                float angle = actualProgress * 90f;
                mapParent.position = centerObject.position + Quaternion.AngleAxis(angle, Vector3.forward) * (startPosition - centerObject.position);
                mapParent.rotation = startRotation * Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    void StartRotation(float angle)
    {
        startPosition = mapParent.position;
        startRotation = mapParent.rotation;
        rotationProgress = 0f;
        isRotating = true;
    }
}