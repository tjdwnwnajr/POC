using UnityEngine;

public class handCheck : MonoBehaviour
{
    
    public float grabY;
    public float grabX;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        Vector2 contactPoint =
            col.ClosestPoint(transform.position);
        grabY = contactPoint.y;
        grabX = contactPoint.x;

        Debug.Log("Á¢ÃË ¿ùµå ÁÂÇ¥: " + contactPoint);
    }
}
