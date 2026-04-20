using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    public GameObject speechBubble;
    public float detectDistance = 3.0f;
    [SerializeField] private bool onlyOnce = false; // 인스펙터에서 온/오프 선택
    private Transform player;
    private bool hasShown = false; // 최초 1회 표시 여부

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (onlyOnce)
        {
            // 아직 한번도 안보여줬으면
            if (!hasShown)
            {
                if (distance <= detectDistance)
                {
                    speechBubble.SetActive(true);
                    hasShown = true;
                }
            }
            else
            {
                // 한번 보여준 이후에는 범위 벗어나면 끄기만 함
                if (distance > detectDistance)
                {
                    if (speechBubble.activeSelf)
                        speechBubble.SetActive(false);
                }
            }
        }
        else
        {
            // 오프 : 기존 방식대로 작동
            if (distance <= detectDistance)
            {
                if (!speechBubble.activeSelf)
                    speechBubble.SetActive(true);
            }
            else
            {
                if (speechBubble.activeSelf)
                    speechBubble.SetActive(false);
            }
        }
    }
}