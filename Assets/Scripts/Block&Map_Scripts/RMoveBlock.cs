using UnityEngine;

public class RMoveBlock : MonoBehaviour
{
    private bool canMove = false;
    [SerializeField] private float moveSpeed = 3f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private Transform limitLTransform;
    [SerializeField] private Transform limitRTransform; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        getDirection();
        
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void getDirection()
    {
        moveDirection = new Vector2(InputManager.BlockMovement.x, 0f);
    }
    private void Move()
    {
        if (canMove)
        {
            Vector2 velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;
            Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
            Vector2 targetPos = rb.position + velocity;
            Mathf.Clamp(targetPos.x, limitLTransform.position.x, limitRTransform.position.x);
            rb.MovePosition(targetPos);
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canMove = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canMove = false;
        }
    }
}
