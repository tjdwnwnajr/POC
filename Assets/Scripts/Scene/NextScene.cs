using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    [SerializeField] private SceneField nextScene;
    [SerializeField] private float BrightOutSpeed = 1f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float BrightInSpeed = 1f;

    public void SwapSceneFromCutScene()
    {
       StartCoroutine(BrightOutAndChangeScene());
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
}
