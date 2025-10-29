using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
//조절 예정 변수
// walkSpeed, jumpPower, maxAirjumps, jumpBufferFrames, coyoteTime, dashSpeed, dashTime, dashCooldown, timeBetweenAttack,
// sideAttackArea, upAttackArea, downAttackArea, damage
{   // 이동관련 변수
    [Header("Move Controller")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]
    private float xAxis;

    //점프 관련 변수
    [Header("Jump Controller")]
    [SerializeField] private float jumpPower=45; //점프 강도
    [SerializeField] private Transform groundCheckPoint; 
    [SerializeField] private float groundCheckDistanceY = 0.2f; //바닥 체크 x,y범위 
    [SerializeField] private float groundCheckDistanceX = 0.5f;
    [SerializeField] private LayerMask groundlayer;
    

    
    private int airJumpCounter = 0; //공중 점프 카운트 변수
    [SerializeField] private int maxAirJumps; //최대 공중 점프 횟수
    [Space(5)]

    //스테이트리스트 관련
    [HideInInspector] public PlayerStateList pState;
    //점프 버퍼 : 점프를 미리 눌러도 점프가 작동하도록 하는 변수
    private int jumpBufferCounter = 0;
    [Header("StateList")]
    [SerializeField] private int jumpBufferFrames;

    //coyote time : 땅에서 떨어져도 점프가 작동하도록 하는변수 
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

    //카메라 스크립트에서 사용하려고
    public static PlayerController Instance;

    

    //////////////////공격 관련 
    [SerializeField]private float timeBetweenAttack; //공격 속도 제한
    private float timeSinceAttack;
    private float yAxis;
    [SerializeField] private Transform sideAttackTrans, upAttackTrans, downAttackTrans;
    [SerializeField] private Vector2 sideAttackArea, upAttackArea, downAttackArea; //공격 범위
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private float damage;
    [SerializeField] private GameObject slashEffect;
    [Space(5)]

    /////반동관련 부분
    [Header("Recoil")]
    [SerializeField] private float recoilXSpeed = 100; //반동속도
    [SerializeField] private float recoilYSpeed = 100;
    [SerializeField] private int recoilXSteps = 5;
    [SerializeField] private int recoilYSteps = 5; //반동 지속시간
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
        //이동,애니메이션을 위한 초기화
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


    //사용자함수
    /////////////////////////////////////////////////////////
    
    //이동방향 입력함수
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
    
    //이동 함수
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

        if (Grounded())//땅에 있으면 바로 대쉬가능하도록 dashed false
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

    //////////////점프 관련///////////////////////
  
     bool Grounded() //땅이면 true아니면 false
    {   
        //좌우 발 밑, 가운데 밑이 땅인지 확인
        if(Physics2D.Raycast(groundCheckPoint.position,Vector2.down, groundCheckDistanceY, groundlayer) || Physics2D.Raycast(groundCheckPoint.position+new Vector3(groundCheckDistanceX,0,0), Vector2.down, groundCheckDistanceY, groundlayer) || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckDistanceX, 0, 0), Vector2.down, groundCheckDistanceY, groundlayer))
        {
            return true;
        }
        return false;
    }


    void Jump() 
    {   
        //점프 캔슬
        if (Input.GetButtonUp("Jump") && rb.linearVelocityY > 0)
        {
            rb.linearVelocityY = 0;

            //pState.jumping = false;
        }

        //if (pState.jumping != true)
        //{
        //    //점프
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

    //플레이어 상태가 접지상태인지 확인하는 함수
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



    ///////////////////////////////공격.....

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
        _slashEffect.transform.eulerAngles = new Vector3(0,0,_effectAngle); //이펙트 종횡 방향 설정
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y); //방향에 맞게 좌우 -1값 바뀌는 것


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
            airJumpCounter = 0; //수정 가능 공격시 점프 가능횟수 초기화
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
        health = Mathf.Clamp(health,0,maxhealth);//현재 체력,최소, 최대

    }
    IEnumerator StopTakingDamgae()
    {
        pState.invincible = true;
        ClampHealth();
        anim.SetTrigger("takeDamage");
        yield return new WaitForSeconds(1f); //무적시간 조절
        pState.invincible = false;

    }
}
