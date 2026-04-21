using Cinemachine;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class OpenLastDoor : TriggerInteractionBase
{

    [SerializeField] private SpriteRenderer doorRenderer;
    private bool isTriggered = false;

    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile profile;
    [SerializeField] private bool isDoor = false;

    [Header("No Key - Dialogue & Death")]

    [Tooltip("열쇠가 없을 때 표시할 대사 목록 (순서대로 페이드인)")]
    [SerializeField]
    private string[] noKeyDialogues = new string[]
{
    "꿈의 열쇠를 찾아와라.",
    "그렇지 않은 자에게는",
    "오직...죽음 뿐."
};

    [Tooltip("마지막 대사 후 사망 처리까지 대기 시간")]
    [SerializeField] private float deathDelayAfterDialogue = 1.0f;
    [SerializeField] private Animator anim;

    [Tooltip("사망 후 이동할 씬")]
    [SerializeField] private SceneField sceneAfterDeathOne;
    [SerializeField] private SceneField sceneAfterDeathTwo;

    [Tooltip("이동할 씬에서 스폰될 문 위치")]
    [SerializeField]
    private DoorTriggerInteraction.DoorToSpawnAt spawnDoorAfterDeath
        = DoorTriggerInteraction.DoorToSpawnAt.One;

    [Header("Door Locked - Dialogue")]
    [SerializeField] private TextMeshProUGUI doorLockedText;

    [SerializeField]
    private string[] doorLockedDialogues = new string[]
    {
    "먼저 문을 여시오."
    };

    [Header("Open Door - Animation")]
    [SerializeField] private Animator doorAnim;
    [SerializeField] private GameObject doorminimap;
    [SerializeField] private NextScene sceneChange;

    [Header("UISettings")]
    [SerializeField] private UITriggerObject uiObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        if (!isDoor)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }
            
    }
    public override void Interact()
    {
        
        if (!isDoor)
        {
            CheckWhichKeyIsNotReady();
            if (PlayerStateList.keyReady && !isTriggered)
            {
                InputManager.DeactivatePlayerControls();
                isTriggered = true;
                
               

                
                if (doorRenderer != null)
                    StartCoroutine(doorOff());
                
            }
            else if (PlayerStateList.keyReady == false && !isTriggered)
            {
                InputManager.DeactivatePlayerControls();
                isTriggered = true;
                //열쇠 없음 ui나오게
                StartCoroutine(NoKeySequence());
            }
            
        }
        else
        {
            if (CheckDoorOpen()&&!isTriggered)
            {
                InputManager.DeactivatePlayerControls();
                isTriggered = true;
                StartCoroutine(BirghtOutAndTheEnd());
            }
            else
            {
                InputManager.DeactivatePlayerControls();
                DoorDialogueSystem.instance.ChangeText(doorLockedText);
                DoorDialogueSystem.instance.StartDialogueWithLines(
                    doorLockedDialogues,
                    () => InputManager.ActivatePlayerControls()
                );
            }
        }
        
    }

    private bool CheckDoorOpen()
    {
        if (doorRenderer != null)
        {
            if (doorRenderer.color.a == 0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    private void CheckWhichKeyIsNotReady()
    {
        if(PlayerStateList.firstKeyFounded == false)
        {
            spawnDoorAfterDeath = DoorTriggerInteraction.DoorToSpawnAt.Two;
        }
        else if(PlayerStateList.secondKeyFounded == false)
        {
            //두번째 열쇠 안찾음
            sceneAfterDeathOne = sceneAfterDeathTwo;
            spawnDoorAfterDeath = DoorTriggerInteraction.DoorToSpawnAt.One;
        }
        else if(PlayerStateList.thirdKeyFounded == false)
        {
            //세번째 열쇠 안찾음
            sceneAfterDeathOne = sceneAfterDeathTwo;
            spawnDoorAfterDeath = DoorTriggerInteraction.DoorToSpawnAt.One;
        }
    }
    private IEnumerator VibrateTimes()
    {
        DualSenseInput.Instance.Vibrate(0.1f, 0.05f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.1f, 0.1f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.15f, 0.15f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.2f, 0.2f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.5f, 0.4f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.6f, 0.6f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.2f, 0.2f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.1f, 0.1f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        DualSenseInput.Instance.Vibrate(0.05f, 0.1f, 0.3f);

    }
    private IEnumerator ActivateControll()
    {
        yield return new WaitForSeconds(3f);
        InputManager.ActivatePlayerControls();
    }
    private IEnumerator doorOff()
    {
        SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.key, transform, 0.5f);
        //yield return new WaitForSeconds(3f);
        CameraEventManager.instance.CameraShakeEvent(profile, impulseSource);
        StartCoroutine(VibrateTimes());
        Color a = doorRenderer.color;
        float t = 0;
        while (t < 3f)
        {
            t += Time.deltaTime;
            float per = t / 3f;
            a.a = Mathf.Lerp(1f, 0f, per);

            doorRenderer.color = a;

            yield return null;
        }

        doorRenderer.color = new Color(a.r, a.g, a.b, 0f);
        if(doorminimap != null)
            doorminimap.SetActive(false);
        uiObj.isDone = true;
        StartCoroutine(ActivateControll());
    }
    private IEnumerator BirghtOutAndTheEnd() {
        InputManager.DeactivatePlayerControls();
        uiObj.isDone = true;
        if(doorAnim != null)
        {
            doorAnim.SetTrigger("isOpen");
        }
        SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.door, transform, 0.8f);
        yield return new WaitForSeconds(4f);

        sceneChange.SwapSceneFromCutScene();
        //SceneBrightManager.instance.StartBrightOut();

        ////keep fading out
        //while (SceneBrightManager.instance.IsBrightOut)
        //{
        //    yield return null;
        //}
        //yield return new WaitForSeconds(1f);

        //SceneManager.LoadScene("TheEndScene");

        //SceneBrightManager.instance.StartBrightIn();
    }

    private IEnumerator NoKeySequence()
    {
        // 1) 대사 표시 - 완료될 때까지 대기
        bool dialogueDone = false;

        if (DoorDialogueSystem.instance != null && noKeyDialogues.Length > 0)
        {
            DoorDialogueSystem.instance.StartDialogueWithLines(
                noKeyDialogues,
                () => dialogueDone = true
            );

            yield return new WaitUntil(() => dialogueDone);
        }

        // 2) 대사 후 짧은 대기
        yield return new WaitForSeconds(deathDelayAfterDialogue);

        // 3) 플레이어 사망
        // 죽음 애니메이션 재생
        anim.SetTrigger("isDie");

        // 4) 사망 애니메이션 후 씬 이동 (Die() 내부 Respawn 전에 씬을 바꿈)
        yield return new WaitForSeconds(2.0f);
        isTriggered = false;

        // 5) 씬 이동
        SceneSwapManager.SwapSceneFromDoorUse(sceneAfterDeathOne, spawnDoorAfterDeath);
        anim.SetTrigger("isAlive");
        
        
    }
}
