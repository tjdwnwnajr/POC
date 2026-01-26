using UnityEngine;

public class handCheck : MonoBehaviour
{
    
    public float grabY;
    public float grabX;
    public Vector2 handPos;
    public bool canGrab;
    public Rigidbody2D hangedRope;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 7)
        {
            //Vector2 contactPoint = 
            //    collision.ClosestPoint(transform.position);
            //grabY = contactPoint.y;
            //grabX = contactPoint.x;
            handPos = transform.position;
            hangedRope = collision.gameObject.GetComponent<Rigidbody2D>();
            canGrab = true;
            
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        canGrab = false;
        handPos = new Vector2(0, 0);
        hangedRope = null;
    }
}
