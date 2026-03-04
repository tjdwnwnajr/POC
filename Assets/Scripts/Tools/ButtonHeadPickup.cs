using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonHeadPickup : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;

    private bool playerInRange;

    private void OnEnable()
    {
        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;

        PlayerStateList.headBtn = true;

        gameObject.SetActive(false); // ÁÝ°í »ç¶óÁü
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
}