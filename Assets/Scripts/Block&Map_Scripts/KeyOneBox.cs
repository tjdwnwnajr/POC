using UnityEngine;
using UnityEngine.InputSystem;

public class KeyOneBox : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private GameObject getKeyUI; // 추가

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

            ShowUI(); // 변경된 부분

            gameObject.SetActive(false);
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

    private void ShowUI()
    {
        if (getKeyUI != null)
            getKeyUI.SetActive(true);
    }
}