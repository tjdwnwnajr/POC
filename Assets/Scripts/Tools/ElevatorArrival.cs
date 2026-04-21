using System.Collections;
using UnityEngine;
using Cinemachine;

public class ElevatorArrival : MonoBehaviour
{
    [Header("Elevator")]
    [SerializeField] private Transform elevatorParent;  // ElevatorParent 오브젝트
    [SerializeField] private float startOffsetY = -10f; // 시작 위치 (아래에서 시작)
    [SerializeField] private float riseDuration = 2f;   // 올라오는 시간

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile arrivalShake;

    private void Start()
    {
        StartCoroutine(ArrivalSequence());
    }

    private IEnumerator ArrivalSequence()
    {
        // ── 1. 초기 세팅
        InputManager.DeactivatePlayerControls();
        PlayerStateList.isView = true;
        PlayerStateList.mapRotation = true;
        // 플레이어를 ElevatorParent 자식으로 (같이 올라오게)
        PlayerController.Instance.transform.position = elevatorParent.position + new Vector3(-4.4702f, -2f, 0f); // 1f는 엘리베이터 위
        PlayerController.Instance.transform.SetParent(elevatorParent);

        // 카메라를 ElevatorParent Follow
        vcam.Follow = elevatorParent;

        // 도착 위치 저장 후 시작 위치로 이동
        Vector3 arrivalPos = elevatorParent.position;
        Vector3 startPos = arrivalPos + new Vector3(0f, startOffsetY, 0f);
        elevatorParent.position = startPos;

        // ── 2. 페이드 인 (암전 → 밝아짐)
        SceneFadeManager.instance.StartFadeIn();
        yield return new WaitForSeconds(0.3f);
        SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.arrive, transform, 0.8f);
        // ── 3. 엘리베이터 상승
        float elapsed = 0f;
        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / riseDuration);
            elevatorParent.position = Vector3.Lerp(startPos, arrivalPos, t);
            yield return null;
        }

        elevatorParent.position = arrivalPos;
        PlayerStateList.mapRotation = false;
        // ── 4. 도착 연출 (진동 + 흔들림)
        DualSenseInput.Instance?.Vibrate(0.7f, 1.0f, 0.5f);
        CameraEventManager.instance.CameraShakeEvent(arrivalShake, impulseSource);

        yield return new WaitForSeconds(0.4f);

        // ── 5. 플레이어 제어권 반환
        PlayerController.Instance.transform.SetParent(null);
        vcam.Follow = PlayerController.Instance.transform;

        InputManager.ActivatePlayerControls();
        PlayerStateList.isView = false;
    }
}