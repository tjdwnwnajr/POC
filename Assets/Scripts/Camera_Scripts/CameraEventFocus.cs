using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraEventFocus : MonoBehaviour
{
    [Header("Virtual Camera")]
    public CinemachineVirtualCamera vcam;

    [Header("Move Settings")]
    public float moveDuration = 0.5f;   // 이동 시간
    public float maxOffsetDistance = 6f; // 너무 멀어지는 것 방지

 
    private CinemachineCameraOffset offset;
    private Vector3 defaultOffset;
    private Coroutine moveCoroutine;

    void Awake()
    {
        // Framing Transposer 가져오기
        
        offset = vcam.GetComponent<CinemachineCameraOffset>();

        if (offset == null)
        {
            Debug.LogError("cameraoffset이 없습니다.");
            enabled = false;
            return;
        }


        // 기본 오프셋 저장
        
        defaultOffset = offset.m_Offset;
    }

    /* ===============================
     * 이벤트 위치로 카메라 시선 이동
     * =============================== */
    public void FocusEvent(Transform startPos, Transform eventTarget)
    {
        if (eventTarget == null) return;

        Vector3 dir = eventTarget.position - startPos.position;
        dir.z = 0f; // 2D 기준 (3D면 제거)

        // 거리 제한
        //if (dir.magnitude > maxOffsetDistance)
        //    dir = dir.normalized * maxOffsetDistance;

        StartMove(dir);
    }

    /* ===============================
     * 다시 플레이어 중심으로 복귀
     * =============================== */
    public void ReturnToPlayer()
    {
        StartMove(defaultOffset);
    }

    /* ===============================
     * 내부 이동 처리
     * =============================== */
    private void StartMove(Vector3 targetOffset)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveOffset(targetOffset));
    }

    private IEnumerator MoveOffset(Vector3 targetOffset)
    {
        Vector3 startOffset = offset.m_Offset;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            offset.m_Offset =
                Vector3.Lerp(startOffset, targetOffset, t);
            yield return null;
        }

        offset.m_Offset = targetOffset;
    }
}
