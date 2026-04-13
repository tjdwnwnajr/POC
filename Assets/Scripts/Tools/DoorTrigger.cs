using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class DoorTrigger : MonoBehaviour
{
    public static bool isOpened = true;
    [SerializeField] private GameObject doorB;
    [SerializeField] private GameObject keyUsePanel;
    [SerializeField] private GameObject noKeyPanel;
    [SerializeField] private GameObject yesButton;
    [SerializeField] private GameObject noButton;
    private bool playerInRange = false;
    private bool isPanelOpen = false;
    private bool isKeyPanel = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (PlayerStateList.keyOne)
            {
                keyUsePanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(yesButton);
                isPanelOpen = true;
                isKeyPanel = true;
                PlayerStateList.canMove = false;
                PlayerStateList.isView = true;
            }
            else
            {
                noKeyPanel.SetActive(true);
                isPanelOpen = true;
                isKeyPanel = false;
                PlayerStateList.canMove = false;
                PlayerStateList.isView = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            ClosePanels();
        }
    }

    private void Update()
    {
        if (!isPanelOpen) return;
        if (Gamepad.current == null) return;

        // noKeyPanel일때는 West버튼으로 닫기만 함
        if (!isKeyPanel)
        {
            if (Gamepad.current.buttonWest.wasPressedThisFrame)
                ClosePanels();
            return;
        }

        // West버튼으로 현재 선택된 버튼 확정
        if (Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            // EventSystem에서 현재 선택된 오브젝트가 무엇인지 확인
            GameObject selected = EventSystem.current.currentSelectedGameObject;

            if (selected == yesButton)
                OnClickYes();
            else if (selected == noButton)
                OnClickNo();
        }
    }

    private void ClosePanels()
    {
        isPanelOpen = false;
        isKeyPanel = false;
        keyUsePanel.SetActive(false);
        noKeyPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        PlayerStateList.canMove = true;
        PlayerStateList.isView = false;
    }

    public void OnClickYes()
    {
        if (!playerInRange) return;

        ClosePanels();

        PlayerStateList.keyOne = false;
        gameObject.SetActive(false);

        if (doorB != null)
            doorB.SetActive(true);
    }

    public void OnClickNo()
    {
        ClosePanels();
    }
}