using UnityEngine;
using UnityEngine.EventSystems;
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject soundSettingMenu;
    [SerializeField] private PlayerController player;

    [Header("Fist selected Options")]
    [SerializeField] private GameObject mainMenuFirst;
    [SerializeField] private GameObject settingMenuFirst;

    private bool isPaused;

    private void Start()
    {
        mainMenu.SetActive(false);
        soundSettingMenu.SetActive(false);
       
    }

    private void Update()
    {
        if (InputManager.settingOpenPressed)
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
        
    }

    private void Pause()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<PlayerController>();
        isPaused = true;
        Time.timeScale = 0f;
        player.enabled = false;

        OpenMainMenu();
    }

    private void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1f;
        player.enabled = true;

        CloseAllMenu();
    }

    private void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        soundSettingMenu.SetActive(false);
        

        EventSystem.current.SetSelectedGameObject(mainMenuFirst);   
    }

    private void CloseAllMenu()
    {
        mainMenu.SetActive(false);
        soundSettingMenu.SetActive(false);
        
    }

    private void OpenSettingMenuHandle()
    {
        mainMenu.SetActive(false);
        soundSettingMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingMenuFirst);
    }

    public void OpenSettingPress()
    {
        OpenSettingMenuHandle();
    }

    public void OnResumePress()
    {
        Unpause();
    }

    public void OnSettingBackPress()
    {
        OpenMainMenu();
    }
}
