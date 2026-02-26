using UnityEngine;
using UnityEngine.InputSystem;

public class FakeWallButton : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction; // Inspector ÁöÁ¤
    [SerializeField] private WallFadeOut targetWall;

    private bool playerInRange = false;
    private bool pressed = false;

    private void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerInRange) return;
        if (pressed) return;

        pressed = true;

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
}