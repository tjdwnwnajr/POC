using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetandExit : MonoBehaviour
{
    
    public static void ResetGame()
    {
        //inputmanager reset
        InputManager.ResetInputValues();
        DualSenseInput.Instance = null;

        //cameramanager reset
        CameraEventManager.instance = null;
        CameraShakeManager.instance = null;
        CameraEventFocus.instance = null;

        //SceneSwapManager reset
        SceneSwapManager.ResetSceneSwapManager();
        SceneSwapManager.Instance = null;

        //MapManager reset
        MapManager.instance = null;

        //SoundFxManager reset 
        SoundFXManager.instance = null;

        //MenuManager reset
        MainMenuManager.instance.Unpause();

        //Fade/Bright Manager reset
        SceneFadeManager.instance = null;
        SceneBrightManager.instance = null;

        //playerstate reset
        PlayerStateList.ResetPlayerState();

        

        GameObject[] ddolObjects = GameObject.FindGameObjectsWithTag("DDOL");

        foreach (GameObject obj in ddolObjects)
        {
            Destroy(obj);
        }
        SceneManager.LoadScene("MainMenu");
    }
}
