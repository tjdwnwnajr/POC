using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public static Vector3 savePoint;
    public static Transform savePointParent;
    
    private void SetSavePoint()
    {
        if (transform.parent !=null)
        {
            savePointParent = transform.parent;
            savePoint = transform.localPosition;
        }
        else
        {
            savePointParent = null;
            savePoint = transform.position;
        }
            //savePoint = newPos;
    }
    public static Vector3 GetRespawnPosition()
    {
        
        if (savePointParent != null)
            return savePointParent.TransformPoint(savePoint);

        
        return savePoint;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector3 newPos = transform.position;
            SetSavePoint();
        }
    }
}
