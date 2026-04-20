using System.Collections;
using UnityEngine;
using Cinemachine;

public class ElevatorActivate : TriggerInteractionBase
{
    [SerializeField] private SceneField nextScene;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile shakeProfile;

    private bool activated = false;

    public override void Interact()
    {
        if (activated) return;
        activated = true;
        StartCoroutine(ActivateSequence());
    }

    private IEnumerator ActivateSequence()
    {
        InputManager.DeactivatePlayerControls();
        PlayerStateList.isView = true;

        DualSenseInput.Instance?.Vibrate(0.9f, 1.0f, 0.4f);
        CameraEventManager.instance.CameraShakeEvent(shakeProfile, impulseSource);
        yield return new WaitForSeconds(2f);
        SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.up, transform, 0.8f);
        // 2초 대기
        

        SceneFadeManager.instance.StartFadeOut();
        
        yield return new WaitUntil(() => !SceneFadeManager.instance.IsFadingOut);
        yield return new WaitForSeconds(2f);
        SceneSwapManager.SwapSceneFromDoorUse(nextScene,
            DoorTriggerInteraction.DoorToSpawnAt.None,false,false);
    }
}