using UnityEngine;
using TMPro; // TextMeshPro를 사용한다면 추가

public class NPCInteraction : MonoBehaviour
{
    public GameObject speechBubble; // 만든 Canvas(말풍선) 연결
    public Transform player;         // Player 오브젝트 연결
    public float detectDistance = 3.0f; // 말풍선이 뜰 거리

    void Update()
    {
        // 1. 플레이어와 NPC 사이의 거리 계산
        float distance = Vector2.Distance(transform.position, player.position);

        // 2. 일정 거리 안에 들어오면 활성화, 멀어지면 비활성화
        if (distance <= detectDistance)
        {
            if (!speechBubble.activeSelf) speechBubble.SetActive(true);

            // [선택] 말풍선이 항상 앞면을 보게 하려면 (필요시)
            // speechBubble.transform.rotation = Quaternion.identity;
        }
        else
        {
            if (speechBubble.activeSelf) speechBubble.SetActive(false);
        }
    }
}