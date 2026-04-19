using UnityEngine;
using System.Collections;
using UnityEditor.Animations;

public class MirrorDevice : TriggerInteractionBase
{
    [SerializeField] private Transform mirrorSpawnPoint;
    [SerializeField] private bool mirrorOn = true;
    private AnimatorController basicController;
    private AnimatorController mirrorController;
    private Animator playerAnim;
    protected override void Start()
    {
        base.Start();
        playerAnim = Player.GetComponent<Animator>();
        basicController = playerAnim.runtimeAnimatorController as AnimatorController;
        mirrorController = Resources.Load<AnimatorController>("Animators/PlayerMirrorController");
    }
    public override void Interact()
    {
        StartCoroutine(EnterMirror());
    }

    private IEnumerator EnterMirror()
    {
        InputManager.DeactivatePlayerControls();

        GameObject player = PlayerController.Instance.gameObject;
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        
        // 1.5초에 걸쳐 투명하게
        float elapsed = 0f;
        float duration = 1.5f;
        SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.mirrorOn, transform, 0.5f);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            if (sr != null)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            yield return null;
        }

        // 순간이동 및 미러 활성화

        player.transform.position = mirrorSpawnPoint.position;
        PlayerStateList.isMirror = mirrorOn;
        if (mirrorOn)
            playerAnim.runtimeAnimatorController = mirrorController;
        else playerAnim.runtimeAnimatorController = basicController;
        
        yield return new WaitForSeconds(0.5f); // 잠시 대기

        // 다시 불투명하게
        elapsed = 0f;
        SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.mirrorOff, transform, 0.5f);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            if (sr != null)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            yield return null;
        }

        if (sr != null)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

        InputManager.ActivatePlayerControls();
    }
}