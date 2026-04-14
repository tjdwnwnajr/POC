using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonInteract : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private GameObject buttonHeadVisual;
    [SerializeField] private WallFadeOut targetWall;
    [SerializeField] private GameObject needHeadUI;

    private bool playerInRange;
    private bool isFixed = false;

    private void Start()
    {
        if (WorldStateManager.Instance != null && WorldStateManager.Instance.buttonFixed)
        {
            isFixed = true;

            if (buttonHeadVisual != null)
                buttonHeadVisual.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
            interactAction.action.Disable();
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;

        if (!isFixed)
        {
            if (!PlayerStateList.headBtn)
            {
                ShowUI();
                return;
            }

            PlayerStateList.headBtn = false;
            isFixed = true;

            if (WorldStateManager.Instance != null)
                WorldStateManager.Instance.buttonFixed = true;

            if (buttonHeadVisual != null)
                buttonHeadVisual.SetActive(true);

            return;
        }

        if (targetWall != null)
            targetWall.FadeOut();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    private void ShowUI()
    {
        if (needHeadUI != null)
        {
            needHeadUI.SetActive(true);
            Debug.Log("UI 켜짐");
        }
    }
}