using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public static Vector3 savePoint;
    
    private void SetSavePoint(Vector3 newPos)
    {
        savePoint = newPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector3 newPos = transform.position;
            SetSavePoint(newPos);
        }
    }
}
