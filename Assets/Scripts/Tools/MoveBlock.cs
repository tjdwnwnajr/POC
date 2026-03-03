using Cinemachine;
using System.Collections;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    [Header("Block Settings")]
    [SerializeField] private Transform[] blocksToMove;           // 이동시킬 블럭
    [SerializeField] private Transform[] targetBlocks;
    [SerializeField] private float moveDuration = 0.5f;       // 이동 시간 (초)

    [Header("Mode Settings")]
    [SerializeField] private bool isBtnMode = false;          // true: 토글 모드, false: 홀드 모드

    [Header("Shake Settings")]
    [SerializeField] private bool shakeOn = false;               // 이동 시 화면 흔들림 여부
    [SerializeField] private float shakeDuration = 0.2f;        // 흔들림 지속 시간
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile profile;

    [Header("Camera Moving")]
    [SerializeField] private float cameraDuration = 0.15f;
    private Vector3[] originalPositions;
    private int currentBlockIndex = 0;

    private Coroutine moveCoroutine;
    private bool[] isMovedArray;                    // 현재 목표위치 상태인지 추적
    private bool canPress = true;                             // 토글 모드에서 중복 입력 방지
    private bool isPlayerOnButton = false;                    // 플레이어가 버튼 위에 있는지 추적
    private bool allBlocksMoved = false;

 

    private void Start()
    {
        if(shakeOn)
            impulseSource = GetComponent<CinemachineImpulseSource>();
        if(blocksToMove != null&& blocksToMove.Length > 0)
        {
            originalPositions = new Vector3[blocksToMove.Length];
            isMovedArray = new bool[blocksToMove.Length];

            for(int i = 0; i< blocksToMove.Length; i++)
            {
                originalPositions[i] = blocksToMove[i].position;
                isMovedArray[i] = false;
            }
        }
    }

    private void Update()
    {
        // isBtnMode일 때 입력으로 작동 (플레이어가 버튼 위에 있을 때만)
        if (isBtnMode && isPlayerOnButton && InputManager.UseToolWasPressed)
        {
            
            HandleToggleMode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // isBtnMode일 때는 충돌로 작동하지 않음
        if (isBtnMode)
            return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Rock"))
        {
            // 윗면에서만 충돌했는지 확인
            if (!IsCollidingFromTop(collision))
                return;

            // 홀드 모드
            HandleHoldModeEnter();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // isBtnMode일 때는 충돌로 작동하지 않음
        if (isBtnMode)
            return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Rock"))
        {
       
            // 홀드 모드에서만 떨어질 때 처리
            HandleHoldModeExit();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // isBtnMode일 때만 Trigger 감지
        if (!isBtnMode)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnButton = true;
           
        }
        if (collision.gameObject.CompareTag("Rock"))
        {
            StartCoroutine(MovebyRock());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // isBtnMode일 때만 처리
        if (!isBtnMode)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnButton = false;
            
        }
    }

    private void HandleToggleMode()
    {
        if (!canPress) return;

        canPress = false;

        // 기존 코루틴 멈추기
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        // 토글: 목표위치 상태면 원래위치로, 아니면 목표위치로
        if (allBlocksMoved)
        {
            moveCoroutine = StartCoroutine(ReturnAllBlocksSequentially());
            allBlocksMoved = false;
        }
        else
        {
            moveCoroutine = StartCoroutine(MoveAllBlocksSequentially());
            allBlocksMoved = true;
        }
    }
    private IEnumerator MoveAllBlocksSequentially()
    {
        InputManager.DeactivatePlayerControls();
        for (int i = 0; i < blocksToMove.Length; i++)
        {
            currentBlockIndex = i;
            CameraEventManager.instance.CameraOffsetEvent(transform, blocksToMove[currentBlockIndex], shakeDuration + moveDuration, false, cameraDuration);
            yield return StartCoroutine(MoveBlockToTarget());
            yield return new WaitForSeconds(1f);
        }
        InputManager.ActivatePlayerControls();
        canPress = true;
    }

    private IEnumerator ReturnAllBlocksSequentially()
    {
        InputManager.DeactivatePlayerControls();
        for (int i = 0; i < blocksToMove.Length; i++)
        {
            currentBlockIndex = i;
            CameraEventManager.instance.CameraOffsetEvent(transform, blocksToMove[currentBlockIndex], shakeDuration + moveDuration, false, cameraDuration); //카메라 기본위치에서 움직이는 블록으로 움직이는 동안 
            yield return StartCoroutine(MoveBlockToOriginal());
            yield return new WaitForSeconds(1f);
        }
        InputManager.ActivatePlayerControls();
        canPress = true;
    }
    private void HandleHoldModeEnter()
    {
        
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveBlockToTarget());
    }

    private void HandleHoldModeExit()
    {
    
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveBlockToOriginal());
    }

    private IEnumerator MoveBlockToTarget()
    {
        if (currentBlockIndex >= targetBlocks.Length)
        {
            Debug.LogError("Target block index out of range.");
            yield break;
        }
        
        Transform blockToMove = blocksToMove[currentBlockIndex];
        Vector3 startPosition = blockToMove.position;
        Vector3 targetPosition = Vector3.zero;
        if(currentBlockIndex < targetBlocks.Length)
        {
            targetPosition = targetBlocks[currentBlockIndex].position;
        }
        else
        {
            Debug.LogError("Target block index out of range.");
            yield break;
        }

        float elapsed = 0f;

        if (shakeOn)
        {
            CameraEventManager.instance.CameraShakeEvent(profile, impulseSource);
            DualSenseInput.Instance.Vibrate(0.15f, 0.05f, shakeDuration+moveDuration);
        
            yield return new WaitForSeconds(shakeDuration);
        }

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            blockToMove.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        blockToMove.position = targetPosition;
        


    }

    private IEnumerator MoveBlockToOriginal()
    {
        if (currentBlockIndex >= targetBlocks.Length)
        {
            Debug.LogError("Target block index out of range.");
            yield break;
        }

        Transform blockToMove = blocksToMove[currentBlockIndex];
        Vector3 startPosition = blockToMove.position;
        Vector3 originalPosition = originalPositions[currentBlockIndex];
        
 
        float elapsed = 0f;
        if (shakeOn)
        {
            CameraEventManager.instance.CameraShakeEvent(profile, impulseSource);
            DualSenseInput.Instance.Vibrate(0.15f, 0.05f, shakeDuration+moveDuration);
            //CameraEventManager.instance.CameraOffsetEvent(transform, blockToMove, shakeDuration + moveDuration);
            yield return new WaitForSeconds(shakeDuration);
        }

            while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            blockToMove.position = Vector3.Lerp(startPosition, originalPosition, t);
            yield return null;
        }

        blockToMove.position = originalPosition;

    }
   
    private bool IsCollidingFromTop(Collision2D collision)
    {
        // 충돌점들을 확인
        foreach (ContactPoint2D contact in collision.contacts)
        {
           
            if (contact.normal.y < -0.5f)
            {
                return true;
            }
        }
        return false;
    }
    private IEnumerator MovebyRock()
    {
        yield return new WaitForSeconds(1f);
        InputManager.DeactivatePlayerControls();
        for (int i = 0; i < blocksToMove.Length; i++)
        {
            currentBlockIndex = i;
            DualSenseInput.Instance.Vibrate(0.15f, 0.05f, shakeDuration + moveDuration);
            CameraEventManager.instance.CameraOffsetEvent(PlayerController.Instance.transform, blocksToMove[currentBlockIndex], shakeDuration + moveDuration, false, cameraDuration);
            yield return StartCoroutine(MoveBlockToTarget());
            yield return new WaitForSeconds(1f);
        }
        InputManager.ActivatePlayerControls();
        
    }
}