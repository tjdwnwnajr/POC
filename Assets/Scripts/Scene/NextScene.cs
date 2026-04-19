using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    [SerializeField] private SceneField nextScene;
    [SerializeField] private float BrightOutSpeed = 1f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float BrightInSpeed = 1f;
    [SerializeField] private bool selectBrightOut = false;


    public void SwapSceneFromCutScene()
    {
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
}
