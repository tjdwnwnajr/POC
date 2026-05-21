using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    
    [SerializeField] private SceneField nextScene;
    [SerializeField] private float BrightOutSpeed = 1f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float BrightInSpeed = 1f;
    [SerializeField] private bool selectBrightOut = false;
    [SerializeField] private AudioClip endingclip;
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    public void SwapSceneFromCutScene()
    {
        PlayerStateList.mapRotation = false;
       if (selectBrightOut)
       {
           StartCoroutine(BrightOutAndChangeScene());
       }
       else
        {
            StartCoroutine(FadeOutAndChangeScene());
        }

    }

    IEnumerator BrightOutAndChangeScene()
    {
        SceneBrightManager.instance.ChangeSpeedSettings(BrightOutSpeed, BrightInSpeed);
        SceneBrightManager.instance.StartBrightOut();
        while(SceneBrightManager.instance.IsBrightOut)
        {
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(nextScene);
        SceneBrightManager.instance.StartBrightIn(); 

    }

    IEnumerator FadeOutAndChangeScene()
    {
        SceneFadeManager.instance.ChangeSpeedSettings(BrightOutSpeed, BrightInSpeed);
        SceneFadeManager.instance.StartFadeOut();
        while (SceneFadeManager.instance.IsFadingOut)
        {
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(nextScene);
        SceneFadeManager.instance.StartFadeIn();
        
        
    }
    public void FadeSpeedChange()
    {
        SceneFadeManager.instance.ChangeSpeedSettings(BrightOutSpeed, BrightInSpeed);
    }
    public void FadeOut()
    {
        SceneFadeManager.instance.StartFadeOut();
    }
    public void SceneChange()
    {
        SceneManager.LoadScene(nextScene);
        if (selectBrightOut)
        {
            SceneBrightManager.instance.StartBrightIn();
        }
        else
            SceneFadeManager.instance.StartFadeIn();
    }
    public void BGMChange2Ending()
    {
               BGMplayandstop.instance.SwitchBGMtoEnding();
    }
    public void SwitchBGMtoEnding()
    {
        if (endingclip == null) return;
        GameObject bgmObj = GameObject.FindWithTag("Music");
        audioSource = bgmObj.GetComponent<AudioSource>();

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutThenIn(endingclip, 0f));
    }
    private IEnumerator FadeOutThenIn(AudioClip nextClip, float startTime)
    {
        // 페이드 아웃
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        while (elapsed < 2f)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / 2f);
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
        while (elapsed < 2f)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, startVolume, elapsed / 2f);
            yield return null;
        }
        audioSource.volume = startVolume;
        fadeCoroutine = null;
    }
}
