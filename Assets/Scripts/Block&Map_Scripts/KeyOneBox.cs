using UnityEngine;
using UnityEngine.InputSystem;

public class KeyOneBox : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction; // Inspector¿¡¼­ ÁöÁ¤

    private bool playerInRange = false;
    private bool alreadyTaken = false;

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.Disable();
    }

    private void Update()
    {
        if (!playerInRange || alreadyTaken) return;

        if (interactAction.action.triggered)
        {
            PlayerStateList.keyOne = true;
            alreadyTaken = true;

            Debug.Log("¿­¼è 1 È¹µæ!");
        }
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
