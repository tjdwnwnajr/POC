using System.Collections;
using UnityEngine;
using Cinemachine;

public class ElevatorActivate : TriggerInteractionBase  // MonoBehaviour 대신 이걸로 변경
{
    [SerializeField] private GameObject interactPanel;
    [SerializeField] private SceneField nextScene;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile shakeProfile;

    private bool activated = false;

    // TriggerInteractionBase가 트리거 감지를 알아서 해줌
    // OnTriggerEnter/Exit 직접 안써도 됨

    public override void Interact()  // Use 버튼 눌렀을 때 실행
    {
        if (activated) return;
        activated = true;
        StartCoroutine(ShowPanelDelay()); // 바로 띄우는 대신 코루틴 실행
    }
    private IEnumerator ShowPanelDelay()
    {
        yield return new WaitForSeconds(1.5f); // 2초 대기
        interactPanel.SetActive(true);       // 그 다음 UI 표시
    }

    // Yes 버튼에 연결
    public void OnClickActivate()
    {
        interactPanel.SetActive(false);
        StartCoroutine(ActivateSequence());
    }

    // No 버튼에 연결
    public void OnClickCancel()
    {
        activated = false;
        interactPanel.SetActive(false);
    }

    private IEnumerator ActivateSequence()
    {
        InputManager.DeactivatePlayerControls();
        PlayerStateList.isView = true;

        DualSenseInput.Instance?.Vibrate(0.9f, 1.0f, 0.4f);
        CameraEventManager.instance.CameraShakeEvent(shakeProfile, impulseSource);

        yield return new WaitForSeconds(0.5f);

        SceneFadeManager.instance.StartFadeOut();
        yield return new WaitUntil(() => !SceneFadeManager.instance.IsFadingOut);

        SceneSwapManager.SwapSceneFromDoorUse(nextScene,
            DoorTriggerInteraction.DoorToSpawnAt.None);
    }
}