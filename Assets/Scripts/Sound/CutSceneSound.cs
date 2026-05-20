using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DreamBGM : MonoBehaviour
{
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        FadeinSound();
    }

    private void FadeinSound() {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeIn());

    }

    private IEnumerator FadeIn()
    {
        // 페이드 아웃
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        
        audioSource.volume = 0f;
        
        audioSource.loop = true;
        audioSource.Play();
        // 페이드 인
        elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 0.5f, elapsed / 1f);
            yield return null;
        }
        audioSource.volume = 0.5f;

        fadeCoroutine = null;
    }
}