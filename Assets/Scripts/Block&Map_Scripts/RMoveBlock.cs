using UnityEngine;

public class RMoveBlock : MonoBehaviour
{
    private bool canMove = false;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float friction = 0.85f;

    private Vector2 moveDirection;
    
    
    [SerializeField] private Transform limitLTransform;
    [SerializeField] private Transform limitRTransform;

    private Rigidbody2D rb;
    private float currentVelocityX = 0;
    private Vector3 startPos;

    [SerializeField] private Rigidbody2D playerRb;
    private float previousPlatformX;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        previousPlatformX = transform.position.x;
    }
    private void Update()
    {
        getDirection();
        
    }
    private void FixedUpdate()
    {
        
        if (canMove)
        {
            ApplyAcceleration();
            ApplyFriction();
            Move();
            SyncPlayerVelocity();
            
        }
        previousPlatformX = transform.position.x;
    }
    private void getDirection()
    {
        moveDirection = new Vector2(InputManager.BlockMovement.x, 0f);
        
    }
    private void ApplyAcceleration()
    {
        float accelerationX = moveDirection.x * acceleration;
        currentVelocityX += accelerationX * Time.fixedDeltaTime;
        currentVelocityX = Mathf.Clamp(currentVelocityX, -maxSpeed, maxSpeed);

    }

    private void ApplyFriction()
    {
        if(Mathf.Abs(moveDirection.x) < 0.1f)
        {
            currentVelocityX *= friction;
            if (Mathf.Abs(currentVelocityX) < 0.01f)
            {
                currentVelocityX = 0;
            }
        }
    }
    private void Move()
    {
        
        if (canMove)
        {
            Vector3 newPos = transform.position;
            newPos.x += currentVelocityX * Time.fixedDeltaTime;
            if(limitLTransform !=null && limitRTransform !=null)
            {
                float leftLimit = limitLTransform.position.x;
                float rightLimit = limitRTransform.position.x;
                newPos.x = Mathf.Clamp(newPos.x, leftLimit, rightLimit);
            }
            rb.MovePosition(newPos);
        }
        
    }

    private void SyncPlayerVelocity()
    {
        // 플레이어가 발판 위에 있으면 발판 속도에 플레이어 입력 속도 더하기
        if (playerRb != null&&canMove)
        {
            Vector2 playerVel = playerRb.linearVelocity;

            float platformMovement = transform.position.x - previousPlatformX;
            
            float platformVelocity = platformMovement / Time.fixedDeltaTime;

            // 플레이어 입력 속도 + 발판 속도
            playerVel.x += platformVelocity;
            playerRb.linearVelocity = playerVel;
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
