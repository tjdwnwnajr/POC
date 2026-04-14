using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    public GameObject speechBubble;
    public float detectDistance = 3.0f;

    private Transform player;

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