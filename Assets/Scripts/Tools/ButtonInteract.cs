using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonInteract : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private GameObject buttonHeadVisual;
    [SerializeField] private WallFadeOut targetWall;
    [SerializeField] private GameObject needHeadUI; // 추가

    private bool playerInRange;
    private bool isFixed = false;

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

        // 아직 버튼 수리 안됨
        if (!isFixed)
        {
            if (!PlayerStateList.headBtn)
            {
                ShowUI(); // Debug.Log 대신 UI
                return;
            }

            // 버튼 머리 장착
            PlayerStateList.headBtn = false;
            isFixed = true;

            if (buttonHeadVisual != null)
                buttonHeadVisual.SetActive(true);

            return;
        }

        // 수리 완료 → 버튼 작동
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

    private void ShowUI()
    {
        if (needHeadUI != null)
            needHeadUI.SetActive(true);
    }
}