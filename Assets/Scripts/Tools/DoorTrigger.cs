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

    private void Start()
    {
        if (WorldStateManager.Instance != null && WorldStateManager.Instance.doorOpened)
        {
            ApplyOpenedState();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (WorldStateManager.Instance != null && WorldStateManager.Instance.doorOpened) return;

        playerInRange = true;

        if (PlayerStateList.keyOne)
        {
            if (keyUsePanel != null)
                keyUsePanel.SetActive(true);

            if (EventSystem.current != null && yesButton != null)
                EventSystem.current.SetSelectedGameObject(yesButton);

            isPanelOpen = true;
            isKeyPanel = true;
            PlayerStateList.canMove = false;
            PlayerStateList.isView = true;
        }
        else
        {
            if (noKeyPanel != null)
                noKeyPanel.SetActive(true);

            isPanelOpen = true;
            isKeyPanel = false;
            PlayerStateList.canMove = false;
            PlayerStateList.isView = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        ClosePanels();
    }

    private void Update()
    {
        if (!isPanelOpen) return;
        if (Gamepad.current == null) return;

        if (!isKeyPanel)
        {
            if (Gamepad.current.buttonWest.wasPressedThisFrame)
                ClosePanels();
            return;
        }

        if (Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            GameObject selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;

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

        if (keyUsePanel != null)
            keyUsePanel.SetActive(false);

        if (noKeyPanel != null)
            noKeyPanel.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        PlayerStateList.canMove = true;
        PlayerStateList.isView = false;
    }

    public void OnClickYes()
    {
        if (!playerInRange) return;

        ClosePanels();

        PlayerStateList.keyOne = false;

        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.doorOpened = true;

        ApplyOpenedState();
    }

    public void OnClickNo()
    {
        ClosePanels();
    }

    private void ApplyOpenedState()
    {
        gameObject.SetActive(false);

        if (doorB != null)
            doorB.SetActive(true);
    }
}