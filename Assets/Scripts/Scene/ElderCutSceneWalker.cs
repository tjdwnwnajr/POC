using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// ���� �ƾ� - ������ �� �����ʿ��� �ɾ�� ���ߴ� ������Ʈ
/// 
/// [Unity ���� ���]
/// 1. ���� ������Ʈ�� �� ������Ʈ �߰�
/// 2. PlayableDirector�� �ִ� ������Ʈ�� SignalReceiver�� ����
/// 3. Timeline Signal Track:
///    - "OldManStartWalk" �� OldManCutsceneWalker.StartWalking()
///    - "OldManStopWalk"  �� OldManCutsceneWalker.StopWalking()
///    - "OldManEnterDoor" �� OldManCutsceneWalker.EnterDoor()
/// </summary>
public class ElderCutsceneWalker : MonoBehaviour
{
    [Header("Walk Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private bool walkLeft = true;
    [SerializeField] private float walkTerm = 0.4f;
    [SerializeField] private float appearDelay = 0.5f;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private BoxCollider2D col;

    [Header("Stop Points")]
    [SerializeField] private Transform[] stopPoints; // �ν����Ϳ��� ������� ����
    [SerializeField] private float stopDistance = 0.1f;
    private int currentStopIndex = 0;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private bool isWalking = false;


    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();   
        col = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {   
        
        col.enabled = false;
        isWalking = false;
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        SetFacingDirection();
        StartCoroutine(PlayWalkSoundLoop());
    }

    private void FixedUpdate()
    {
        if (!isWalking) return;

        float direction = walkLeft ? -1f : 1f;
        rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocityY);


        if (currentStopIndex < stopPoints.Length)
        {
            float distX = Mathf.Abs(transform.position.x - stopPoints[currentStopIndex].position.x);
            if (distX <= stopDistance)
            {
                StopWalking();
                currentStopIndex++;
            }
        }
    }

    public void AppearElder()
    {
        StartCoroutine(Appear());
    }
    public void DisappearElder()
    {
        StartCoroutine(Disappear());
    }
    IEnumerator Appear()
    {
        
        col.enabled = true;
        
        float duration = appearDelay;
        float elapsed = 0f;
        float alpha = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }
    IEnumerator StartSecondDirector()
    {
        while (isWalking)
        {
            yield return null;
        }
        director.Play();
    }
    IEnumerator Disappear()
    {
       
        float duration = 4f;
        float elapsed = 0f;
        float alpha = 1f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
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
        // ��ȭ�� Timeline Signal�� ���� ó��
    }

    public void KeepWalking()
    {
        isWalking = true;
        anim.SetBool("isWalk", true);
        StartCoroutine(StartSecondDirector());
    }
    public void EnterDoor()
    {
        StartCoroutine(Entering());
    }
    IEnumerator Entering()
    {
        //isWalking = true;
        anim.SetBool("isWalk", true);
        rb.gravityScale = 0f;
        DisappearElder();
        float goalY = 131f;
        float goalX = -70f;
        float duration = 3f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newY = Mathf.Lerp(startPos.y, goalY, elapsed / duration);
            float newX = Mathf.Lerp(startPos.x, goalX, elapsed / duration);
            transform.position = new Vector3(newX, newY, transform.position.z);
            if(newX <= goalX + 0.2f)
            {
                anim.SetBool("isWalk", false);
            }
            yield return null;
        }

        transform.position = new Vector3(goalX, goalY, transform.position.z);
        

    }
   

    private void SetFacingDirection()
    {
        if (walkLeft)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        else
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private IEnumerator PlayWalkSoundLoop()
    {
        while (true)
        {
            if (isWalking && SoundFXManager.instance != null)
            {
                SoundFXManager.instance.PlaySoundFXClip(SoundFXManager.SFX.walk, transform, 0.3f);
            }
            yield return new WaitForSeconds(walkTerm);
        }
    }
    public void PauseDirector(PlayableDirector d)
    {
        d.Pause();
    }
    public void ResumeDirector(PlayableDirector d)
    {
        d.Resume();
    }
}