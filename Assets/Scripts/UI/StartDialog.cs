using UnityEngine;
using UnityEngine.InputSystem;

public class StartDialog : MonoBehaviour
{
    [SerializeField] private DialogSystem dialogSystem;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private Transform dialogStartPoint;

    private PlayerInput playerInput;
    private InputAction useAction;
    private Rigidbody2D playerRb;

    private bool isPlayerInside = false;
    private bool hasAutoStartedOnce = false;
    private bool pendingAutoStart = false;

    private void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        useAction = playerInput.actions["Use"];
        playerRb = player.GetComponent<Rigidbody2D>();

        interactionUI.SetActive(false);
    }

    private void Update()
    {
        if (!hasAutoStartedOnce && pendingAutoStart && player.Grounded() && !dialogSystem.isDialogActive)
        {
            pendingAutoStart = false;
            hasAutoStartedOnce = true;
            interactionUI.SetActive(false);
            StartDialogSequence();
            return;
        }

        if (hasAutoStartedOnce && isPlayerInside && !dialogSystem.isDialogActive)
        {
            interactionUI.SetActive(true);

            if (useAction.WasPressedThisFrame())
            {
                interactionUI.SetActive(false);
                StartDialogSequence();
            }
        }
        else
        {
            interactionUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInside = true;

        if (!hasAutoStartedOnce)
        {
            pendingAutoStart = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInside = false;
        interactionUI.SetActive(false);
    }

    private void StartDialogSequence()
    {
        MovePlayerToDialogStartPoint();
        dialogSystem.StartDialog();
    }

    private void MovePlayerToDialogStartPoint()
    {
        if (dialogStartPoint == null) return;

        playerRb.linearVelocity = Vector2.zero;
        player.transform.position = dialogStartPoint.position;
    }
}