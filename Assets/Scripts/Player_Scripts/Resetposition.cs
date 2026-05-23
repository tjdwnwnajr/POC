using UnityEngine;

public class Resetposition : MonoBehaviour
{
    public static Vector3 resetPoint;
    public static Transform resetPointParent;

    private void SetResetPoint()
    {
        if (transform.parent != null)
        {
            resetPointParent = transform.parent;
            resetPoint = transform.localPosition;
        }
        else
        {
            resetPointParent = null;
            resetPoint = transform.position;
        }
        //resetPoint = newPos;
    }
    public static Vector3 GetRespawnPosition()
    {

        if (resetPointParent != null)
            return resetPointParent.TransformPoint(resetPoint);


        return resetPoint;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector3 newPos = transform.position;
            SetResetPoint();
        }
    }
}
