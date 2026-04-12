using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Timeline Signal으로 플레이어를 자동으로 걷게/멈추게 합니다.
/// 
/// [Unity 세팅 방법]
/// 1. Player 오브젝트에 이 컴포넌트 추가
/// 2. PlayableDirector가 있는 오브젝트에 SignalReceiver 추가
/// 3. Timeline에서 Signal Track 생성
/// 4. "StartWalk" 신호 → SignalReceiver → CutsceneWalker.StartWalking() 연결
/// 5. "StopWalk"  신호 → SignalReceiver → CutsceneWalker.StopWalking()  연결
/// </summary>
public class CutsceneWalker : MonoBehaviour
{
    [Header("Walk Settings")]
    [SerializeField] private float walkSpeed = 3f;          // 컷씬 전용 걷기 속도
    [SerializeField] private bool walkRight = true;          // true: 오른쪽, false: 왼쪽
    [SerializeField] private float walkTerm = 0.3f;                         // 발걸음 소리 간격
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    private bool isWalking = false;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
    }

    private void Start()
    {
        // 씬 시작 시 입력 비활성화 (컷씬 자동 시작)
        InputManager.DeactivatePlayerControls();

        // 씬 시작 시 플레이어 바라보는 방향 세팅
        SetFacingDirection();

        PlayCutSceneWalkSound();
    }

    private void FixedUpdate()
    {
        if (!isWalking) return;

        float direction = walkRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocityY);
    }

    public void StartWalking()
    {
        isWalking = true;
        anim.SetBool("isWalk", true);
    }

  
    public void StopWalking()
    {
        isWalking = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocityY);
        anim.SetBool("isWalk", false);

        // 대사 시스템 시작 (DialogueSystem이 씬에 있으면 자동 연결)
        if (DoorDialogueSystem.instance != null)
        {
            DoorDialogueSystem.instance.StartDialogue();
        }
        
    }

    public void StartDie()
    {
        StartCoroutine(DieAndLoadScene());
    }

    private IEnumerator DieAndLoadScene()
    {
        // 입력 차단
        InputManager.DeactivatePlayerControls();

        // 죽음 애니메이션 재생
        anim.SetTrigger("isDie");

        // 모션 끝날 때까지 대기
        yield return new WaitForSeconds(2.5f);


        anim.SetTrigger("isAlive");
        
    }


    private void SetFacingDirection()
    {
        if (PlayerController.Instance == null) return;

        Transform t = PlayerController.Instance.transform;

        if (walkRight)
        {
            // 오른쪽 바라보기 (y rotation = 0)
            t.rotation = Quaternion.Euler(0f, 0f, 0f);
            PlayerStateList.lookingRight = true;
        }
        else
        {
            // 왼쪽 바라보기 (y rotation = 180)
            t.rotation = Quaternion.Euler(0f, 180f, 0f);
            PlayerStateList.lookingRight = false;
        }
    }
    public void PlayCutSceneWalkSound()
    {
        StartCoroutine(PlayWalkSoundOn());
    }

    IEnumerator PlayWalkSoundOn()
    {
        while (true)
        {
            if (isWalking)
            {
                SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.walk, transform, 0.4f);
            }
            yield return new WaitForSeconds(walkTerm); // 발걸음 소리 간격

        }
    }


}