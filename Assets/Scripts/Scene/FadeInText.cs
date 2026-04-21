using UnityEngine;
using TMPro;
using System.Collections;

public class FadeInText : MonoBehaviour
{
    public TextMeshProUGUI text;

    [Header("Timing Settings")]
    public float delayBeforeFade = 1f;   // 시작 전 대기 시간
    public float fadeDuration = 2f;      // 나타나는 데 걸리는 시간
    public float holdDuration = 2f;      // 다 나타난 후 유지 시간

    void Start()
    {
        SetAlpha(0f);
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        // 1. 대기
        yield return new WaitForSeconds(delayBeforeFade);

        // 2. 페이드 인
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            float alpha = Mathf.SmoothStep(0, 1, t);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(1f);

        // 3. 유지
        yield return new WaitForSeconds(holdDuration);
        
        time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            float alpha = Mathf.SmoothStep(1, 0, t);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f);
        // 여기서 추가 연출 가능 (씬 종료 등)
        ResetandExit.ResetGame();
    }

    void SetAlpha(float a)
    {
        Color color = text.color;
        color.a = a;
        text.color = color;
    }
}