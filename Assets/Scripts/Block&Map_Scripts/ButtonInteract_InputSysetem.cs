using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonInteract_InputSystem : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Target Objects")]
    [SerializeField] private GameObject up;
    [SerializeField] private GameObject down;

    private bool playerInRange = false;

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

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerInRange) return;

        ToggleUpDown();
    }

    private void ToggleUpDown()
    {
        bool upActive = up.activeSelf;

        up.SetActive(!upActive);
        down.SetActive(upActive);
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
