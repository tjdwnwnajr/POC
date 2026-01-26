using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 4f;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        Invoke(nameof(DestroySelf), lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void Launch(Vector2 dir)
    {
        rb.linearVelocity = dir.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.TriggerRecoilX();

            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
