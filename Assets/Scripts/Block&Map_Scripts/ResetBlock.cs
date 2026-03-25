using UnityEngine;

public class ResetBlock : MonoBehaviour
{
    private bool hasTriggered = true;
    private Vector3 startPos;
    private Quaternion startRot;
    private bool contact;
    [SerializeField] private bool isForTrigger = false;
    [SerializeField] private Transform targetBlock = null;
    private void Start()
    {
        if (!isForTrigger)
        {
            startPos = transform.position;
            return;
        }
        if (isForTrigger && targetBlock != null)
        {
            startPos = targetBlock.localPosition;
            startRot = targetBlock.localRotation;
            Debug.Log(startPos);
        }

    }

    private void Update()
    {
        if (!isForTrigger)
        {
            if (InputManager.resetPressedTwo && !hasTriggered && !contact)
            {
                transform.position = startPos;
                hasTriggered = true;
            }
            else
            {
                hasTriggered = false;
            }
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
        if (isForTrigger && targetBlock != null)
        {
            targetBlock.localPosition = startPos;
            targetBlock.localRotation = startRot;
        }
    }
}
