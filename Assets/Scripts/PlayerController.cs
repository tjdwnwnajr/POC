using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
//���� ���� ����
// walkSpeed, jumpPower, maxAirjumps, jumpBufferFrames, coyoteTime, dashSpeed, dashTime, dashCooldown, timeBetweenAttack,
// sideAttackArea, upAttackArea, downAttackArea, damage
{   // �̵����� ����
    [Header("Move Controller")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]
    private float xAxis;

    //���� ���� ����
    [Header("Jump Controller")]
    [SerializeField] private float jumpPower=45; //���� ����
    [SerializeField] private Transform groundCheckPoint; 
    [SerializeField] private float groundCheckDistanceY = 0.2f; //�ٴ� üũ x,y���� 
    [SerializeField] private float groundCheckDistanceX = 0.5f;
    [SerializeField] private LayerMask groundlayer;
    

    
    private int airJumpCounter = 0; //���� ���� ī��Ʈ ����
    [SerializeField] private int maxAirJumps; //�ִ� ���� ���� Ƚ��
    [Space(5)]

    //������Ʈ����Ʈ ����
    [HideInInspector] public PlayerStateList pState;
    //���� ���� : ������ �̸� ������ ������ �۵��ϵ��� �ϴ� ����
    private int jumpBufferCounter = 0;
    [Header("StateList")]
    [SerializeField] private int jumpBufferFrames;

    //coyote time : ������ �������� ������ �۵��ϵ��� �ϴº��� 
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;

    //dash
    [Header("Dash Controller")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    private float gravity;
    private bool canDash = true;
    private bool dashed;
    [SerializeField] GameObject dashEffect;
    [Space(5)]
    


    private Rigidbody2D rb;
    private Animator anim;
    //private SpriteRenderer spr;

    //ī�޶� ��ũ��Ʈ���� ����Ϸ���
    public static PlayerController Instance;

    

    //////////////////���� ���� 
    [SerializeField]private float timeBetweenAttack; //���� �ӵ� ����
    private float timeSinceAttack;
    private float yAxis;
    [SerializeField] private Transform sideAttackTrans, upAttackTrans, downAttackTrans;
    [SerializeField] private Vector2 sideAttackArea, upAttackArea, downAttackArea; //���� ����
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private float damage;
    [SerializeField] private GameObject slashEffect;
    [Space(5)]

    /////�ݵ����� �κ�
    [Header("Recoil")]
    [SerializeField] private float recoilXSpeed = 100; //�ݵ��ӵ�
    [SerializeField] private float recoilYSpeed = 100;
    [SerializeField] private int recoilXSteps = 5;
    [SerializeField] private int recoilYSteps = 5; //�ݵ� ���ӽð�
    private int stepsXRecoiled, stepsYRecoiled;

    [Header("Health")]
    [SerializeField] public int maxhealth;
    [SerializeField] public int health;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        health = maxhealth;
        
    }
    void Start()
    {
        //�̵�,�ִϸ��̼��� ���� �ʱ�ȭ
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //spr = GetComponent<SpriteRenderer>();
        pState = GetComponent<PlayerStateList>();
        pState.lookingRight = true;
        pState.invincible = false;
        gravity = rb.gravityScale;    
       
    }

    private void FixedUpdate()
    {
        GetDirection();
        if (pState.dashing) return;
        
        MoveX();
        Recoil();
    }
    // Update is called once per frame
    void Update()
    {
        
        Jump();
        UpdateJumpVariables();
        StartDash();

        GetAttack();
        Attack();
        
    }


    //������Լ�
    /////////////////////////////////////////////////////////
    
    //�̵����� �Է��Լ�
    void GetDirection()
    {

        xAxis = Input.GetAxisRaw("Horizontal");
       
        yAxis = Input.GetAxisRaw("Vertical");
        if (xAxis >0)
        {
            //spr.flipX = xAxis < 0;
            
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = true;
        }
        else if(xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = false;
        }
    }
    
    //�̵� �Լ�
    void MoveX()
    {
        rb.linearVelocity = new Vector2 (xAxis *  walkSpeed, rb.linearVelocityY);
        
        anim.SetBool("isWalk", rb.linearVelocityX != 0);
    }


    void StartDash()
    {

        
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        { 
            StartCoroutine(Dash());
            dashed = true;
            
        }

        if (Grounded())//���� ������ �ٷ� �뽬�����ϵ��� dashed false
        {
            dashed = false;
        
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("isDash");
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;


    }

    //////////////���� ����///////////////////////
  
     bool Grounded() //���̸� true�ƴϸ� false
    {   
        //�¿� �� ��, ��� ���� ������ Ȯ��
        if(Physics2D.Raycast(groundCheckPoint.position,Vector2.down, groundCheckDistanceY, groundlayer) || Physics2D.Raycast(groundCheckPoint.position+new Vector3(groundCheckDistanceX,0,0), Vector2.down, groundCheckDistanceY, groundlayer) || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckDistanceX, 0, 0), Vector2.down, groundCheckDistanceY, groundlayer))
        {
            return true;
        }
        return false;
    }


    void Jump() 
    {   
        //���� ĵ��
        if (Input.GetButtonUp("Jump") && rb.linearVelocityY > 0)
        {
            rb.linearVelocityY = 0;

            //pState.jumping = false;
        }

        //if (pState.jumping != true)
        //{
        //    //����
        //    if (coyoteTimeCounter > 0 && jumpBufferCounter>0)
        //    {
        //        rb.linearVelocity = new Vector3(rb.linearVelocityX, jumpPower);
        //        pState.jumping = true;
        //    }
        //    else if (!Grounded() &&Input.GetButtonDown("Jump")&&airJumpCounter < maxAirJumps)
        //    {
        //        pState.jumping = true;
        //        airJumpCounter++;
        //        rb.linearVelocity = new Vector3(rb.linearVelocityX, jumpPower);
        //    }
        //}
        if (!pState.jumping  && coyoteTimeCounter > 0 && jumpBufferCounter > 0)

        {
            rb.linearVelocity = new Vector3(rb.linearVelocityX, jumpPower);
            pState.jumping = true;
        }
        else if (pState.jumping && Input.GetButtonDown("Jump") && airJumpCounter < maxAirJumps)
        {
           
            airJumpCounter++;
            rb.linearVelocity = new Vector3(rb.linearVelocityX, jumpPower);
        }

        anim.SetBool("isJump", !Grounded());

  
    }

    //�÷��̾� ���°� ������������ Ȯ���ϴ� �Լ�
    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }



    ///////////////////////////////����.....

    void GetAttack()
    {
        pState.attacking = Input.GetMouseButtonDown(0);
    }
    void Attack()
    {
        timeSinceAttack += Time.deltaTime;

        if (pState.attacking && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("isAttack");

            if (yAxis == 0 || yAxis < 0 && Grounded()) {
                Hit(sideAttackTrans, sideAttackArea,ref pState.recoilingX,recoilXSpeed);
                Instantiate(slashEffect, sideAttackTrans);
            }
            else if(yAxis > 0)
            {
                Hit(upAttackTrans, upAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 90, upAttackTrans);
            }
            else if (yAxis < 0&&!Grounded())
            {
                Hit(downAttackTrans, downAttackArea,ref pState.recoilingY, recoilYSpeed); 
                SlashEffectAtAngle(slashEffect,-90, downAttackTrans);
            }
        }

    }
    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0,0,_effectAngle); //����Ʈ ��Ⱦ ���� ����
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y); //���⿡ �°� �¿� -1�� �ٲ�� ��


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTrans.position,sideAttackArea);
        Gizmos.DrawWireCube(upAttackTrans.position, upAttackArea);
        Gizmos.DrawWireCube(downAttackTrans.position, downAttackArea);

    }
    void Hit(Transform _attacktrans, Vector2 _attackarea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attacktrans.position, _attackarea, 0, attackableLayer);

        if (objectsToHit.Length > 0) {
            _recoilDir = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++) {
            if (objectsToHit[i].GetComponent<Enemy>() != null) { }
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized,_recoilStrength);
            }
        }
    }


    


    ////////////////Recoil
    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.linearVelocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.linearVelocity = new Vector2(recoilXSpeed, 0);
            }
        }
        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, recoilYSpeed);
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0; //���� ���� ���ݽ� ���� ����Ƚ�� �ʱ�ȭ
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if(pState.recoilingY&&stepsYRecoiled< recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }
        if (Grounded())
        {
            StopRecoilY();
        }
    }
    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }
    public void TakeDamage(float _damage)
    {
        health -= Mathf.RoundToInt(_damage);

        StartCoroutine(StopTakingDamgae());
    }
    void ClampHealth()
    {
        health = Mathf.Clamp(health,0,maxhealth);//���� ü��,�ּ�, �ִ�

    }
    IEnumerator StopTakingDamgae()
    {
        pState.invincible = true;
        ClampHealth();
        anim.SetTrigger("takeDamage");
        yield return new WaitForSeconds(1f); //�����ð� ����
        pState.invincible = false;

    }
}
