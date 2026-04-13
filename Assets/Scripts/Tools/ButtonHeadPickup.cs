using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonHeadPickup : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private GameObject getHeadUI; // 추가

    private bool playerInRange;

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

        ShowUI(); // 추가

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