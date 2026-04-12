using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// 화면 중앙 자막 스타일 대사 시스템 (페이드인 버전)
/// 
/// [Unity 세팅 방법]
/// 1. Canvas 생성 (Screen Space - Overlay)
/// 2. Canvas 안에 빈 오브젝트 "DialogueSystem" 생성 → 이 컴포넌트 추가
/// 3. Canvas 안에 TextMeshPro - Text 생성 → dialogueText 필드에 연결
///    - Anchor: Middle Center
///    - Alignment: Center
///    - Font Size: 원하는 크기
///    - Color: 흰색
/// </summary>
public class DoorDialogueSystem : MonoBehaviour
{
    public static DoorDialogueSystem instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 2.5f;    // 스르륵 나오는 시간 (길수록 천천히)
    [SerializeField] private float holdDuration = 3.0f;      // 완전히 보인 후 유지 시간
    [SerializeField] private float fadeOutDuration = 1.5f;   // 사라지는 시간
    [SerializeField] private float delayBetween = 0.5f;      // 다음 대사까지 텀

    // 대사 목록
    private string[] dialogues = new string[]
    {
        "꿈의 열쇠를 찾아와라.",
        "그렇지 않은 자에게는",
        "오직...죽음 뿐."
    };

    private int currentIndex = 0;
    private Coroutine dialogueCoroutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        HideImmediate();
    }

    // =============================================
    // 외부에서 호출 (CutsceneWalker.StopWalking)
    // =============================================
    public void StartDialogue()
    {
        currentIndex = 0;
        dialogueText.gameObject.SetActive(true);

        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);

        dialogueCoroutine = StartCoroutine(PlayAllLines());
    }

    // =============================================
    // 내부 로직
    // =============================================
    private IEnumerator PlayAllLines()
    {
        foreach (string line in dialogues)
        {
            yield return StartCoroutine(ShowLine(line));
            yield return new WaitForSeconds(delayBetween);
        }

        // 모든 대사 끝
        yield return new WaitForSeconds(0.5f);
        HideImmediate();
        InputManager.ActivatePlayerControls();
    }

    private IEnumerator ShowLine(string line)
    {
        // 텍스트 세팅, 완전 투명하게 시작
        dialogueText.text = line;
        SetAlpha(0f);

        // 페이드 인 (스르륵 나오기)
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            // EaseIn 커브 적용 (더 자연스럽게)
            SetAlpha(alpha * alpha);
            yield return null;
        }
        SetAlpha(1f);

        // 유지
        yield return new WaitForSeconds(holdDuration);

        // 페이드 아웃 (스르륵 사라지기)
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeOutDuration);
            SetAlpha(alpha * alpha);
            yield return null;
        }
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color c = dialogueText.color;
        c.a = alpha;
        dialogueText.color = c;
    }

    private void HideImmediate()
    {
        dialogueText.text = "";
        SetAlpha(0f);
        dialogueText.gameObject.SetActive(false);
    }
}