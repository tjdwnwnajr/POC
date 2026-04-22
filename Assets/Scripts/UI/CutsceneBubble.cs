using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// 이동 중 플레이어 위에 말풍선 대사를 표시합니다.
/// Timeline Signal → ShowBubble() 연결
/// 
/// [Unity 세팅 방법]
/// 1. CutsceneManager 오브젝트에 이 컴포넌트 추가
/// 2. bubblePrefab : 말풍선 프리팹 연결
/// 3. canvas : 기존 Canvas 연결
/// 4. player : Player 오브젝트 연결
/// 5. textChildName : 프리팹 안의 TMP 오브젝트 이름 입력
/// 6. Timeline Signal Track에 ShowBubble 신호 연결
/// </summary>
public class CutsceneBubble : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bubblePrefab;   // 말풍선 프리팹
    [SerializeField] private Canvas canvas;              // 기존 Canvas
    [SerializeField] private Transform player;           // Player Transform

    [Header("Settings")]
    [SerializeField] private string textChildName = "Text"; // 프리팹 안 TMP 오브젝트 이름
    [SerializeField] private Vector2 offset = new Vector2(0f, 2.5f); // 플레이어 위 오프셋
    [SerializeField] private float displayDuration = 2.5f;  // 말풍선 표시 시간
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    // 대사 목록 (Timeline Signal 순서대로)
    [Header("Dialogues")]
    [SerializeField]
    private string[] bubbleDialogues = new string[]
    {
        "여기가 맞나...",   // 첫 번째 ShowBubble 신호
        "저 문이다."        // 두 번째 ShowBubble 신호
    };

    private int dialogueIndex = 0;
    private GameObject currentBubble;
    private Camera mainCam;
    private void Awake()
    {
        mainCam = Camera.main;

        if (player == null && PlayerController.Instance != null)
            player = PlayerController.Instance.transform;
    }

    // =============================================
    // Timeline Signal → ShowBubble 에 연결
    // =============================================
    public void ShowBubble()
    {
        if (dialogueIndex >= bubbleDialogues.Length) return;

        string line = bubbleDialogues[dialogueIndex];
        dialogueIndex++;

        // 이전 말풍선 있으면 즉시 제거
        if (currentBubble != null)
        {
            StopAllCoroutines();
            Destroy(currentBubble);
        }
        
        StartCoroutine(SpawnBubble(line));
    }

    // =============================================
    // 내부 로직
    // =============================================
    private IEnumerator SpawnBubble(string line)
    {
        // 프리팹 생성 (Canvas 자식으로)
        currentBubble = Instantiate(bubblePrefab, canvas.transform);

        // TMP 텍스트 찾아서 대사 세팅
        TextMeshProUGUI tmp = currentBubble.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = line;

        // 캔버스그룹으로 페이드 처리
        CanvasGroup cg = currentBubble.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = currentBubble.AddComponent<CanvasGroup>();

        cg.alpha = 0f;

        // 페이드 인
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            UpdateBubblePosition();
            yield return null;
        }
        cg.alpha = 1f;

        // 표시 유지 (이 동안 위치 계속 업데이트)
        elapsed = 0f;
        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;
            UpdateBubblePosition();
            yield return null;
        }

        // 페이드 아웃
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1f - Mathf.Clamp01(elapsed / fadeOutDuration);
            UpdateBubblePosition();
            yield return null;
        }

        Destroy(currentBubble);
        currentBubble = null;
        
    }

    // 월드 좌표 → 스크린 좌표 → Canvas 로컬 좌표로 변환해서 위치 업데이트
    private void UpdateBubblePosition()
    {
        if (currentBubble == null || player == null) return;

        // World Space Canvas면 그냥 월드 좌표로 직접 배치
        currentBubble.transform.position = player.position + new Vector3(offset.x, offset.y, 0f);
    }
    public void Skip()
    {
        StopAllCoroutines();
        dialogueIndex = bubbleDialogues.Length;
    }
}