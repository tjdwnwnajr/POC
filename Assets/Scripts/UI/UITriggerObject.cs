using UnityEngine;
using UnityEngine.InputSystem;

public class UITriggerObject : MonoBehaviour
{
    [SerializeField] private GameObject uiObject;
    [SerializeField] private GameObject interactionUI;
    
    private bool hasShownOnce = false;
    private bool playerInRange = false;
    private MoveBlock moveBlock;
    private void Awake()
    {
        if(uiObject ==null && interactionUI == null)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Canvas"))
                {
                    uiObject = child.gameObject;
                    interactionUI = child.gameObject;
                    break;
                }
            }
        }
        HideUI();
        HideInteractionUI(); // null이어도 안전
        moveBlock = GetComponent<MoveBlock>();
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (!hasShownOnce)
        {
            ShowUI();
            hasShownOnce = true;
        }
        else
        {
            ShowInteractionUI(); // null이면 아무 일도 안 함
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        HideUI();
        HideInteractionUI(); // null 안전
    }

    private void Update()
    {
        if(moveBlock != null)
        {
            if(moveBlock.isUsed == true)
            {
                HideUI();
                HideInteractionUI();
                return;
            }
        }
        if (!hasShownOnce) return;
        if (!playerInRange) return;

        bool pressed = false;

        if (Gamepad.current != null)
            pressed |= Gamepad.current.buttonWest.wasPressedThisFrame;

        pressed |= Input.GetMouseButtonDown(0);

        if (pressed)
        {
            HideInteractionUI(); // null 안전
            ShowUI();
        }
    }

    private void ShowUI()
    {
        if (uiObject != null)
            uiObject.SetActive(true);
    }

    private void HideUI()
    {
        if (uiObject != null)
            uiObject.SetActive(false);
    }

    private void ShowInteractionUI()
    {
        if (interactionUI != null)
            interactionUI.SetActive(true);
    }

    private void HideInteractionUI()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }
}