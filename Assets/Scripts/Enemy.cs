using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isReoiling = false;
    protected float recoilTimer;
    protected Rigidbody2D rb;
    [SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;
    [SerializeField] protected float damage;

    protected virtual void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
        if (recoilTimer < recoilLength)
        {
            recoilTimer += Time.deltaTime;
        }
        else
        {
            isReoiling = false;
            recoilTimer = 0;
        }

    }
    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isReoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isReoiling=true;
        }
    }
    protected void OnTriggerStay2D(Collider2D _other)
    {
        
        if (_other.CompareTag("Player")&&!PlayerController.Instance.pState.invincible)
        {
            Attack();
        }
    }
    protected virtual void Attack() {
        PlayerController.Instance.TakeDamage(damage);
    }
}
