using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BGMplayandstop : MonoBehaviour
{
    public static BGMplayandstop instance { get; private set; }
    [SerializeField] private AudioClip clip1;
    [SerializeField] private AudioClip clip2;
    [SerializeField] private AudioClip endingclip;
    [SerializeField] private string[] bgm2Scenes;
    [SerializeField] private float fadeDuration = 3f;

    private AudioSource audioSource;
    //재생중인지
    private bool isPlaying = false;
    // clip2가 활성화되어 있는지
    private bool isBgm2Active = false;
    // clip2에서 clip1으로 돌아갈 때 재생 위치 저장
    private float bgm2SavedTime = 0f;
    private float bgm1SavedTime = 0f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Update()
    {
        // StartScene에서만 재생시작
        if (!isPlaying && SceneManager.GetActiveScene().name == "StartScene")
        {
            //최초 재생
            audioSource.clip = clip1;
            audioSource.loop = true;
            audioSource.volume = 0.2f;
            audioSource.Play();
            isPlaying = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // BGM이 재생 중이지 않으면 아무 작업도 하지 않음
        if (!isPlaying) return;
        // 현재 씬이 clip2용 씬인지 확인
        bool shouldBeBgm2 = IsBgm2Scene(scene.name);

        //clip1 ->clip2
        if (shouldBeBgm2 && !isBgm2Active)
        {
            bgm1SavedTime = audioSource.time;
            isBgm2Active = true;
            SwitchBGM(clip2, 0f);
        }
        else if (!shouldBeBgm2 && isBgm2Active)
        {
            // clip2 → clip1 (재생 위치 저장)
            bgm2SavedTime = audioSource.time;
            isBgm2Active = false;
            SwitchBGM(clip1, bgm1SavedTime);
        }
    }

    public void SwitchBGMtoEnding()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutThenIn(endingclip, 0f));
    }
    private void SwitchBGM(AudioClip nextClip, float startTime)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOutThenIn(nextClip, startTime));
    }

    private IEnumerator FadeOutThenIn(AudioClip nextClip, float startTime)
    {
        // 페이드 아웃
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();

        // 클립 교체 후 재생
        audioSource.clip = nextClip;
        audioSource.time = startTime;
        audioSource.Play();

        // 페이드 인
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f,startVolume, elapsed / fadeDuration);
            yield return null;
        }
        audioSource.volume = startVolume;
        fadeCoroutine = null;
    }

    private bool IsBgm2Scene(string sceneName)
    {
        if (bgm2Scenes == null) return false;
        foreach (string s in bgm2Scenes)
            if (s == sceneName) return true;
        return false;
    }
    
}