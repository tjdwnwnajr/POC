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
    [SerializeField] private float fadeInDuration = 2.5f;
    [SerializeField] private float holdDuration = 3.0f;
    [SerializeField] private float fadeOutDuration = 1.5f;
    [SerializeField] private float delayBetween = 0.5f;
    private TextMeshProUGUI currentText;
    // 기본 대사 (인스펙터에서 교체 가능)
    [SerializeField]private string[] dialogues = new string[]
    {
        "꿈의 열쇠를 찾아와라.",
        "그렇지 않은 자에게는",
        "오직...죽음 뿐."
    };

    private Coroutine dialogueCoroutine;

    // 대사 완료 시 호출할 콜백
    private System.Action onComplete;

    private void Awake()
    {

        if (instance == null)
            instance = this;
        
        currentText = dialogueText;
        HideImmediate();
    }

    // =============================================
    // 기존 방식: 내부 대사로 시작 (콜백 없음)
    // =============================================
    public void StartDialogue()
    {
        StartDialogueWithLines(dialogues, null);
    }

    // =============================================
    // 신규: 외부 대사 배열 + 완료 콜백으로 시작
    // OpenLastDoor 등 외부에서 호출
    // =============================================
    public void StartDialogueWithLines(string[] lines, System.Action callback)
    {
        onComplete = callback;
        currentText.gameObject.SetActive(true);

        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);

        dialogueCoroutine = StartCoroutine(PlayAllLines(lines));
    }

    // TMP 교체 메서드
    public void ChangeText(TextMeshProUGUI newText)
    {
        HideImmediate();
        currentText = newText;
    }

    // 복구 메서드 (PlayAllLines 끝날 때 자동 호출)
    private void RestoreDefaultText()
    {
        HideImmediate();
        currentText = dialogueText;
    }

    // =============================================
    // 내부 로직
    // =============================================
    private IEnumerator PlayAllLines(string[] lines)
    {
        foreach (string line in lines)
        {
            yield return StartCoroutine(ShowLine(line));
            yield return new WaitForSeconds(delayBetween);
        }

        yield return new WaitForSeconds(0.5f);

        RestoreDefaultText();

        // 콜백이 있으면 콜백 호출, 없으면 기존처럼 컨트롤 활성화
        if (onComplete != null)
            onComplete.Invoke();
        else
            InputManager.ActivatePlayerControls();
    }

    private IEnumerator ShowLine(string line)
    {
        currentText.text = line;
        SetAlpha(0f);

        // 페이드 인
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            SetAlpha(alpha * alpha);
            yield return null;
        }
        SetAlpha(1f);

        // 유지
        yield return new WaitForSeconds(holdDuration);

        // 페이드 아웃
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
        Color c = currentText.color;
        c.a = alpha;
        currentText.color = c;
    }

    private void HideImmediate()
    {
        currentText.text = "";
        SetAlpha(0f);
        currentText.gameObject.SetActive(false);
    }
}