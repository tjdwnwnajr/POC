using UnityEngine;

public class RStickPlatform : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 20f;
    [SerializeField] private float inputDeadZone = 0.15f;

    [Header("이동 범위 제한 (선택)")]
    [SerializeField] private bool useMoveLimit = false;
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;

    private float currentVelocity = 0f;
    private bool isPlayerOnTop = false;
    private Transform playerTransform;
    private Rigidbody2D platformRb;

    private void Start()
    {
        platformRb = GetComponent<Rigidbody2D>();
        if (platformRb != null)
            platformRb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Update()
    {
        if (!isPlayerOnTop)
        {
            ApplyDeceleration();
            MovePlatform();
            return;
        }

        float input = InputManager.BlockMovement.x;

        if (Mathf.Abs(input) < inputDeadZone)
            input = 0f;

        if (Mathf.Abs(input) > 0f)
        {
            currentVelocity = Mathf.MoveTowards(
                currentVelocity,
                input * maxSpeed,
                acceleration * Time.deltaTime
            );
        }
        else
        {
            ApplyDeceleration();
        }

        MovePlatform();
    }

    private void ApplyDeceleration()
    {
        currentVelocity = Mathf.MoveTowards(
            currentVelocity, 0f, deceleration * Time.deltaTime
        );
    }

    private void MovePlatform()
    {
        if (Mathf.Approximately(currentVelocity, 0f)) return;

        Vector3 delta = new Vector3(currentVelocity * Time.deltaTime, 0f, 0f);
        Vector3 nextPos = transform.position + delta;

        if (useMoveLimit)
        {
            float leftL = leftLimit.position.x;
            float rightL = rightLimit.position.x;
            nextPos.x = Mathf.Clamp(nextPos.x, leftL, rightL);
            if (nextPos.x <= leftL || nextPos.x >= rightL)
                currentVelocity = 0f;
        }

        transform.position = nextPos;

        if (isPlayerOnTop && playerTransform != null)
            playerTransform.position += delta;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (IsCollidingFromTop(collision))
        {
            isPlayerOnTop = true;
            playerTransform = collision.transform;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (!isPlayerOnTop && IsCollidingFromTop(collision))
        {
            isPlayerOnTop = true;
            playerTransform = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        isPlayerOnTop = false;
        playerTransform = null;
    }

    private bool IsCollidingFromTop(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
                return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!useMoveLimit) return;
        Gizmos.color = Color.cyan;
        Vector3 left  = new Vector3(leftLimit.position.x,  transform.position.y, 0f);
        Vector3 right = new Vector3(rightLimit.position.x, transform.position.y, 0f);
        Gizmos.DrawLine(left  + Vector3.down, left  + Vector3.up);
        Gizmos.DrawLine(right + Vector3.down, right + Vector3.up);
        Gizmos.DrawLine(left, right);
    }
}