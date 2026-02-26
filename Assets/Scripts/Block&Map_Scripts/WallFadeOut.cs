using UnityEngine;
using System.Collections;

public class WallFadeOut : MonoBehaviour
{
    public float fadeDuration = 1.5f;

    private SpriteRenderer[] renderers;
    private bool isFading = false;

    void Awake()
    {
        // 부모 + 자식 모든 SpriteRenderer 가져오기
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void FadeOut()
    {
        if (!isFading)
            StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        isFading = true;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            foreach (var sr in renderers)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }
}