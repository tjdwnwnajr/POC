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

        // 1. 플레이어와 NPC 사이의 거리 계산
        float distance = Vector2.Distance(transform.position, player.position);

        // 2. 사거리 안에 있을 때만 입력 체크
        if (distance <= interactRange)
        {
            // [방법 A] 게임패드 서쪽 버튼
            if (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame)
            {
                dialogSystem.StartDialog();
            }

            // [방법 B] 마우스 우클릭 (화면 어디든!)
            // Raycast 로직을 제거하여 NPC를 직접 클릭하지 않아도 작동합니다.
            if (Input.GetMouseButtonDown(1))
            {
                dialogSystem.StartDialog();
            }
        }
    }
}