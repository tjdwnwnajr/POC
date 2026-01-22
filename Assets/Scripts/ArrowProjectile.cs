using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float lifeTime = 4f;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 dir, float speed)
    {
        rb.linearVelocity = dir.normalized * speed;
        Invoke(nameof(DestroySelf), lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어나 벽에 맞으면 제거
        if (other.CompareTag("Player") || other.CompareTag("Ground"))
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
