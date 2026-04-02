using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // 추가 필수

public class MainMenu : MonoBehaviour
{
    public GameObject firstButton; // 에디터에서 'Start' 버튼을 여기에 드래그하세요.

    void Start()
    {
        // 게임 시작 시 패드/키보드로 조작할 수 있도록 첫 번째 버튼을 활성화
        EventSystem.current.SetSelectedGameObject(null); // 초기화
        EventSystem.current.SetSelectedGameObject(firstButton); // Start 버튼 선택
    }

    public void OnClickStart()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서도 꺼지게 확인용
#else
        Application.Quit();
#endif
    }
}