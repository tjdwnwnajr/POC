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
    [SerializeField] private float interactDistance = 3.0f;

    private Transform player;
    private bool isPanelOpen = false;
    private bool isKeyPanel = false;
    private bool hasTriggeredThisRange = false;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Start()
    {
        if (WorldStateManager.Instance != null && WorldStateManager.Instance.doorOpened)
        {
            ApplyOpenedState();
        }
    }

    private void Update()
    {
        if (player == null) return;
        if (WorldStateManager.Instance != null && WorldStateManager.Instance.doorOpened) return;

        float distance = Vector2.Distance(transform.position, player.position);
        bool inRange = distance <= interactDistance;

        if (inRange)
        {
            if (!hasTriggeredThisRange)
            {
                OpenPanel();
                hasTriggeredThisRange = true;
            }
        }
        else
        {
            hasTriggeredThisRange = false;

            if (isPanelOpen)
                ClosePanels();
        }

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

    private void OpenPanel()
    {
        if (PlayerStateList.keyOne)
        {
            if (keyUsePanel != null)
                keyUsePanel.SetActive(true);

            if (EventSystem.current != null && yesButton != null)
                EventSystem.current.SetSelectedGameObject(yesButton);

            isKeyPanel = true;
        }
        else
        {
            if (noKeyPanel != null)
                noKeyPanel.SetActive(true);

            isKeyPanel = false;
        }

        isPanelOpen = true;
        PlayerStateList.canMove = false;
        PlayerStateList.isView = true;
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