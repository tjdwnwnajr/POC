using UnityEngine;

public class ResetBlock : MonoBehaviour
{
    private bool hasTriggered = true;
    private Vector3 startPos;
    private bool contact;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        Debug.Log(startPos);
        if (InputManager.resetPressedTwo && !hasTriggered&&!contact)
        {
            transform.position = startPos;
            hasTriggered = true;
            Debug.Log("¼º°ø");
        }
        else
        {
            hasTriggered = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            contact = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            contact = false;
        }
    }
}
