using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}