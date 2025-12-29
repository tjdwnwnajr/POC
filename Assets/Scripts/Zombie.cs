using UnityEngine;

public class Zombie : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool isMoving = false;

    private Animator anim;
    protected override void Awake() { 
        base.Awake();
        anim = GetComponent<Animator>();
    }
    protected override void Start()
    {
        rb.gravityScale = 12f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!isReoiling)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
            isMoving = true;

        }
        if (isMoving) {
            anim.SetBool("isWalk", true);
        }
        else if (!isMoving)
        {
            anim.SetBool("isWalk", false);
        }

    }
    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
    }
}
