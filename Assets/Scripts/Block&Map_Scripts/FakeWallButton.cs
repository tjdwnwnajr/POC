using UnityEngine;
using UnityEngine.InputSystem;

public class FakeWallButton : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private WallFadeOut targetWall;

    private bool playerInRange = false;
    private bool pressed = false;

    private void Start()
    {
        if (WorldStateManager.Instance != null && WorldStateManager.Instance.fakeWallOpened)
        {
            pressed = true;

            if (targetWall != null)
                targetWall.FadeOut();
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

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerInRange) return;
        if (pressed) return;

        pressed = true;

        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.fakeWallOpened = true;

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
}