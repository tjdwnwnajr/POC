using UnityEngine;
using System.Collections;

public class MoveBlock : MonoBehaviour
{
    [Header("Block Settings")]
    [SerializeField] private Transform blockToMove;           // 이동시킬 블럭
    [SerializeField] private Transform targetBlock;
    private Vector3 targetPosition;          // 목표 위치
    [SerializeField] private float moveDuration = 0.5f;       // 이동 시간 (초)

    [Header("Mode Settings")]
    [SerializeField] private bool isBtnMode = false;          // true: 토글 모드, false: 홀드 모드


    private Vector3 originalPosition;
    private Coroutine moveCoroutine;
    private bool isMoved = false;                    // 현재 목표위치 상태인지 추적
    private bool canPress = true;                             // 토글 모드에서 중복 입력 방지
    private bool isPlayerOnButton = false;                    // 플레이어가 버튼 위에 있는지 추적

    private void Start()
    {
        if (blockToMove != null)
        {
            originalPosition = blockToMove.position;
            targetPosition = targetBlock.position;
        }
        else
        {
            Debug.LogWarning("이동시킬 블럭이 할당되지 않았습니다!");
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

        if (collision.gameObject.CompareTag("Player"))
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

        if (collision.gameObject.CompareTag("Player"))
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
        if (isMoved)
        {
            moveCoroutine = StartCoroutine(MoveBlockToOriginal());
            isMoved = false;
        }
        else
        {
            moveCoroutine = StartCoroutine(MoveBlockToTarget());
            isMoved = true;
        }
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
        Vector3 startPosition = blockToMove.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            blockToMove.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        blockToMove.position = targetPosition;

        if (isBtnMode)
            canPress = true;
    }

    private IEnumerator MoveBlockToOriginal()
    {
        Vector3 startPosition = blockToMove.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            blockToMove.position = Vector3.Lerp(startPosition, originalPosition, t);
            yield return null;
        }

        blockToMove.position = originalPosition;

        if (isBtnMode)
            canPress = true;
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
}