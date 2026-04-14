using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonHeadPickup : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private GameObject getHeadUI;

    private bool playerInRange;

    private void Start()
    {
        if (WorldStateManager.Instance != null && WorldStateManager.Instance.headBtnTaken)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;

        PlayerStateList.headBtn = true;

        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.headBtnTaken = true;

        ShowUI();
        gameObject.SetActive(false);
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
        if (getHeadUI != null)
            getHeadUI.SetActive(true);
    }
}