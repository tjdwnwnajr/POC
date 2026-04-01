using UnityEngine;
using UnityEngine.InputSystem;

public class DialogNPC : MonoBehaviour
{
    public DialogSystem dialogSystem;
    public Transform player;
    public float interactRange = 2.5f;

    void Update()
    {
        if (player == null || dialogSystem == null) return;

        // 대화 중일 때는 시작 로직 무시
        if (dialogSystem.isDialogActive) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactRange)
        {
            // 1. 게임패드 서쪽 버튼 (X/네모) 상호작용 복구
            if (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame)
            {
                dialogSystem.StartDialog();
            }

            // 2. 마우스 우클릭(1) 상호작용 복구
            if (Input.GetMouseButtonDown(1))
            {
                dialogSystem.StartDialog();
            }
        }
    }
}