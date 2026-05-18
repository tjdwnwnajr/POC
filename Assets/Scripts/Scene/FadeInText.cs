using UnityEngine;
using TMPro;
using System.Collections;

public class FadeInText : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshProUGUI text;              // "THE END" 텍스트

    [Header("Stats Text References")]
    [Tooltip("플레이타임을 표시할 TMP (없으면 생략)")]
    public TextMeshProUGUI playTimeText;

    [Tooltip("사망 횟수를 표시할 TMP (없으면 생략)")]
    public TextMeshProUGUI deathCountText;

    [Tooltip("꿈 3 방문 여부를 표시할 TMP (없으면 생략)")]
    public TextMeshProUGUI dream3Text;

    [Header("THE END Timing")]
    public float delayBeforeFade = 1f;   // THE END 나오기 전 대기
    public float theEndFadeIn = 2f;   // THE END 페이드 인 시간
    public float theEndHold = 2f;   // THE END 유지 시간 (스탯이 올라오는 동안 포함)

    [Header("Stats Timing")]
    public float statsDelayAfterTheEnd = 1f;  // THE END 페이드인 완료 후 스탯 나오기까지 대기
    public float statsFadeIn = 1.5f; // 스탯 페이드 인 시간
    public float statsHold = 3f;   // 스탯 + THE END 같이 유지되는 시간
    public float statsFadeOut = 1.5f; // 스탯 + THE END 동시 페이드 아웃 시간

    // ─────────────────────────────────────────────────────
    void Start()
    {
        SetAlpha(text, 0f);
        SetAlpha(playTimeText, 0f);
        SetAlpha(deathCountText, 0f);
        SetAlpha(dream3Text, 0f);

        // 스탯 텍스트 내용 세팅
        if (playTimeText != null && PlaytimeManager.instance != null)
            playTimeText.text = $"Play Time : {PlaytimeManager.instance.GetFormattedTime()}";

        if (deathCountText != null)
            deathCountText.text = $"Deaths : {PlayerStateList.deathCount}";

        if (dream3Text != null)
        {
            bool visited = SceneSwapManager.isDreamCleared != null
                           && SceneSwapManager.isDreamCleared.Length > 2
                           && SceneSwapManager.isDreamCleared[2];
            dream3Text.text = visited ? "꿈 3    클리어" : "꿈 3    X";
        }

        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        // 1. 대기 후 THE END 페이드 인
        yield return new WaitForSeconds(delayBeforeFade);
        yield return FadeText(text, 0f, 1f, theEndFadeIn);

        // 2. THE END 유지하면서 잠깐 대기 후 스탯 동시 페이드 인
        yield return new WaitForSeconds(statsDelayAfterTheEnd);
        StartCoroutine(FadeText(playTimeText, 0f, 1f, statsFadeIn));
        StartCoroutine(FadeText(deathCountText, 0f, 1f, statsFadeIn));
        StartCoroutine(FadeText(dream3Text, 0f, 1f, statsFadeIn));

        // 3. 스탯 페이드 인이 끝날 때까지 대기
        yield return new WaitForSeconds(statsFadeIn);

        // 4. THE END + 스탯 모두 유지
        yield return new WaitForSeconds(statsHold);

        // 5. THE END + 스탯 동시 페이드 아웃
        StartCoroutine(FadeText(text, 1f, 0f, statsFadeOut));
        StartCoroutine(FadeText(playTimeText, 1f, 0f, statsFadeOut));
        StartCoroutine(FadeText(deathCountText, 1f, 0f, statsFadeOut));
        StartCoroutine(FadeText(dream3Text, 1f, 0f, statsFadeOut));

        yield return new WaitForSeconds(statsFadeOut);

        // 6. 종료
        ResetandExit.ResetGame();
    }

    IEnumerator FadeText(TextMeshProUGUI target, float from, float to, float duration)
    {
        if (target == null) yield break;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.SmoothStep(from, to, Mathf.Clamp01(time / duration));
            SetAlpha(target, alpha);
            yield return null;
        }
        SetAlpha(target, to);
    }

    void SetAlpha(TextMeshProUGUI target, float a)
    {
        if (target == null) return;
        Color c = target.color;
        c.a = a;
        target.color = c;
    }
}