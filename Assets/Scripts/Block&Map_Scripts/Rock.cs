using UnityEngine;

public class RollingStone : MonoBehaviour
{
    [Header("Rolling Settings")]
    [SerializeField] private float rollingTorque = 10f;      // 회전력

    [Header("Destruction")]
    [SerializeField] private float destroyTime = 5f;         // 파괴 시간

    private Rigidbody2D rb;
    private bool isInitialized = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        isInitialized = true;

        // destroyTime 후 자동 파괴
        Destroy(gameObject, destroyTime);
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;

        // 낙하하면서 회전 적용
        ApplyRollingRotation();
    }

    private void ApplyRollingRotation()
    {
        // 수평 이동 속도를 회전으로 변환
        float horizontalVelocity = Mathf.Abs(rb.linearVelocity.x);

        if (horizontalVelocity > 0.1f)
        {
            // 이동 방향에 따라 회전 방향 결정
            float torqueDirection = rb.linearVelocity.x > 0 ? 1f : -1f;
            // 속도에 비례하여 각속도 직접 설정
            rb.angularVelocity = horizontalVelocity * rollingTorque * torqueDirection;
        }
    }
}