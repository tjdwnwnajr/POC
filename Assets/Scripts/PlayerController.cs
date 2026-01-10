using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
//조절 예정 변수
// walkSpeed, jumpPower, maxAirjumps, jumpBufferFrames, coyoteTime, dashSpeed, dashTime, dashCooldown, timeBetweenAttack,
// sideAttackArea, upAttackArea, downAttackArea, damage
{   // 이동관련 변수

    //private CameraTarget _cameraFollowObject;
    //[SerializeField] private GameObject _cameraFollowGo; 
    //Camera manager
    private float _fallSpeedYDampingChangeThreshold;


    //로프 매달리기
    [HideInInspector]public bool isRope = false;
    private FixedJoint2D playerJoint;
    private bool ropeKeyPressed;
    private bool canCatch = false;
    private Rigidbody2D ropeRb;

    private Vector2 moveInput;
    [Header("Move Controller")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]
    private float xAxis;



    //점프 관련 변수
    private bool jumpPressed;
    private bool jumpKeyDown;
    private bool jumpKeyUp;
    [Header("Jump Controller")]
    [SerializeField] private float jumpPower = 45; //점프 강도
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckDistanceY = 0.2f; //바닥 체크 x,y범위 
    [SerializeField] private float groundCheckDistanceX = 0.5f;
    [SerializeField] private LayerMask groundlayer;
    [SerializeField] private float jumpMinTime; //최소 점프 시간
    private float jumpCountTime = 0; //점프 시간 측정 변수
    [SerializeField] private float fallSpeedLimit = -10f;
    //중력 관련
    [SerializeField] float gravityUp = 1.2f;        // 상승 중 기본 중력
    [SerializeField] float gravityHang = 0.3f;      // 정점 체공 중력 (핵심)
    [SerializeField] float gravityFall = 2.0f;      // 낙하 중 중력

    [SerializeField] float hangVelocityThreshold = 0.15f; // 정점 판정
    [SerializeField] float gravitySmoothSpeed = 10f;


    //[SerializeField] private float gravityUp;
    //[SerializeField] private float gravityDefault;
    //[SerializeField] private float gravityCut;

    

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
    private bool dashKeyDown;
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


    //카메라 스크립트에서 사용하려고
    public static PlayerController Instance;



    //////////////////공격 관련 
    private bool attackKeyDown;
    [SerializeField] private float timeBetweenAttack; //공격 속도 제한
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

    //블럭움직이기
    [HideInInspector] public float rotateInput;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance != null && Instance != this)
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
        pState = GetComponent<PlayerStateList>();
        playerJoint = GetComponent<FixedJoint2D>();

        playerJoint.enabled = false;

        pState.lookingRight = true;
        pState.invincible = false;
        gravity = rb.gravityScale;
        //_cameraFollowObject = _cameraFollowGo.GetComponent<CameraTarget>();
        _fallSpeedYDampingChangeThreshold = CameraManager.Instance._fallSpeedYDampingChangeThresholde;
    }

    private void FixedUpdate()
    {
        if (pState.dashing) return;
        if (isRope) return;
        GetDirection();
        
        MoveX();
        Recoil();
        LimitFallSpeed();
        UpdateGravity();
    }
    // Update is called once per frame
    void Update()
    {
        if (isRope)
        {
            ReleaseRope();
            return;
        }
        Jump();
        UpdateJumpVariables();
        StartDash();

        GetAttack();
        Attack();
        CatchRope();
        

        //낙하속도가 임계값보다 작아야함, 카메라 y반응속도가 이미 조절중인 상태가 아니어야함, 이미 낙하상태로 조절된 상태가 아니어야함
        if (rb.linearVelocityY < _fallSpeedYDampingChangeThreshold && !CameraManager.Instance.IsLerpingYDamping && !CameraManager.Instance.LerpedFromPlayerFalling)
        {
            CameraManager.Instance.LerpYDamping(true);
        }
        //낙하중이 아니어야함, 카메라y반응속도가 낙하로 조정되어있어야함,카메라 y반응속도가 조절중인 상태가 아니어야함
        if (rb.linearVelocityY >= 0 && CameraManager.Instance.LerpedFromPlayerFalling && !CameraManager.Instance.IsLerpingYDamping)
        {
            //원래로 복귀
            CameraManager.Instance.LerpYDamping(false);
            //낙하로 조정된 상태가 아님을 알리기 위해 false로 설정
            CameraManager.Instance.LerpedFromPlayerFalling = false;
        }
    }

    void LateUpdate()
    {
        jumpKeyDown = false;
        jumpKeyUp = false;
        dashKeyDown = false;
        attackKeyDown = false;
    }
    //사용자함수
    /////////////////////////////////////////////////////////

    //이동방향 입력함수

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        Debug.Log(moveInput.x);
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
            rotateInput = context.ReadValue<float>();
        else if (context.canceled)
            rotateInput = 0f;

    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            jumpKeyDown = true;
            jumpKeyUp = false;
        }
        if (ctx.canceled)
        {
            jumpKeyUp = true;
            jumpKeyDown = false;
        }
        jumpPressed = ctx.performed;
    }
    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            dashKeyDown = true;
        }
        if (ctx.canceled)
        {
            dashKeyDown = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            attackKeyDown = true;
        }
        if (ctx.canceled)
        {
            attackKeyDown = false;

        }
    }

    public void OnRope(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            ropeKeyPressed = true;

        }
        if (ctx.canceled)
        {
            ropeKeyPressed = false;
        }
    }

    private void Turn()
    {
        if (pState.lookingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            //_cameraFollowObject.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            //_cameraFollowObject.CallTurn();
        }
    }

    void GetDirection()
    {

        ///xAxis = Input.GetAxisRaw("Horizontal");

        //yAxis = Input.GetAxisRaw("Vertical");
        xAxis = moveInput.x;
        yAxis = moveInput.y;
        //if(xAxis == 0) {
        //    xAxis = Input.GetAxisRaw("Horizontal");
        //}
        //if(yAxis == 0)
        //{
        //    yAxis = Input.GetAxisRaw("Vertical");
        //}   

        if (xAxis > 0)
        {
            //spr.flipX = xAxis < 0;

            //transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = true;
            Turn();
        }
        else if (xAxis < 0)
        {
            //transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = false;
            Turn();
        }
    }

    //이동 함수
    void MoveX()
    {
        rb.linearVelocity = new Vector2(xAxis * walkSpeed, rb.linearVelocityY);

        anim.SetBool("isWalk", rb.linearVelocityX != 0);
    }


    void StartDash()
    {


        if (dashKeyDown && canDash && !dashed)
        //if (DualSenseInput.Instance.DashPressed && canDash && !dashed)
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
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistanceY, groundlayer) || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckDistanceX, 0, 0), Vector2.down, groundCheckDistanceY, groundlayer) || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckDistanceX, 0, 0), Vector2.down, groundCheckDistanceY, groundlayer))
        {
            return true;
        }
        return false;
    }


    void Jump()
    {

        //coyoteTime은 땅에서 떨어져도 점프가 작동하도록 하는 변수 jumpBuffer는 점프를 미리 눌러도 점프가 작동하도록 하는변수
        if (!pState.jumping && coyoteTimeCounter > 0 && jumpBufferCounter > 0)

        {
            rb.linearVelocity = new Vector3(rb.linearVelocityX, jumpPower);
            pState.jumping = true;
            jumpCountTime = 0;
        }
        else if (pState.jumping && /*Input.GetButtonDown("Jump")*/ jumpKeyDown && airJumpCounter < maxAirJumps)
        {

            airJumpCounter++;
            rb.linearVelocity = new Vector3(rb.linearVelocityX, jumpPower);
            jumpCountTime = 0;
        }

        //점프 캔슬
        if (pState.jumping)
        {
            jumpCountTime += Time.deltaTime;

            if (jumpKeyUp/*Input.GetButtonUp("Jump")*/ && rb.linearVelocityY > 0)
            {
                if (jumpCountTime < jumpMinTime)
                {
                    StartCoroutine(JumpMinTime(jumpMinTime - jumpCountTime));
                }
                else
                {
                    rb.linearVelocityY = 0;
                }
                //pState.jumping = false;
            }
        }


        anim.SetBool("isJump", !Grounded());


    }
    IEnumerator JumpMinTime(float _time)
    {
        yield return new WaitForSeconds(_time);
        if (rb.linearVelocityY > 0)
        {
            rb.linearVelocityY = 0;
        }
    }
    void LimitFallSpeed()
    {
        // Y속도가 fallSpeedLimit보다 작으면 fallSpeedLimit로 제한
        if (rb.linearVelocityY < fallSpeedLimit)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, fallSpeedLimit);
        }
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

        if (jumpKeyDown)//Input.GetButtonDown("Jump")|| DualSenseInput.Instance.JumpDown)
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    void UpdateGravity()
    {
        float vy = rb.linearVelocityY;

        // 목표 중력
        float targetGravity;

        // 1. 상승 중
        if (vy > hangVelocityThreshold)
        {
            targetGravity = gravityUp;
        }
        // 2. 정점 근처 (체공)
        else if (Mathf.Abs(vy) <= hangVelocityThreshold)
        {
            targetGravity = gravityHang;
        }
        // 3. 낙하 중
        else
        {
            targetGravity = gravityFall;
        }

        // 중력 부드럽게 전환
        rb.gravityScale = Mathf.Lerp(
            rb.gravityScale,
            targetGravity,
            gravitySmoothSpeed * Time.deltaTime
        );
    }


    ///////////////////////////////공격.....

    void GetAttack()
    {
        pState.attacking = attackKeyDown;//Input.GetMouseButtonDown(0);
    }
    void Attack()
    {
        timeSinceAttack += Time.deltaTime;

        if (pState.attacking && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("isAttack");

            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                Hit(sideAttackTrans, sideAttackArea, ref pState.recoilingX, recoilXSpeed);
                Instantiate(slashEffect, sideAttackTrans);
            }
            else if (yAxis > 0)
            {
                Hit(upAttackTrans, upAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 90, upAttackTrans);
            }
            else if (yAxis < 0 && !Grounded())
            {
                Hit(downAttackTrans, downAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, downAttackTrans);
            }
        }

    }
    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle); //이펙트 종횡 방향 설정
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y); //방향에 맞게 좌우 -1값 바뀌는 것


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTrans.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttackTrans.position, upAttackArea);
        Gizmos.DrawWireCube(downAttackTrans.position, downAttackArea);

    }
    void Hit(Transform _attacktrans, Vector2 _attackarea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attacktrans.position, _attackarea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilDir = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null) { }
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrength);
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
        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
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
        if (DualSenseInput.Instance != null)
            DualSenseInput.Instance.Vibrate(0.5f, 1.0f, 0.3f);

        StartCoroutine(StopTakingDamgae());
    }
    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxhealth);//현재 체력,최소, 최대

    }
    IEnumerator StopTakingDamgae()
    {
        pState.invincible = true;
        ClampHealth();
        anim.SetTrigger("takeDamage");
        yield return new WaitForSeconds(1f); //무적시간 조절
        pState.invincible = false;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Rope"))
        {
            ropeRb = collision.gameObject.GetComponent<Rigidbody2D>();
            canCatch = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Rope"))
        {
            ropeRb = null;
            canCatch = false;
        }

    }
   
    private void CatchRope()
    {
        if(canCatch&&ropeKeyPressed && !isRope)
        {
            isRope = true;
            playerJoint.enabled = true;
            playerJoint.connectedBody = ropeRb;
        }
    }
    private void ReleaseRope()
    {
        if(isRope && !ropeKeyPressed)
        {
            isRope = false;
            playerJoint.enabled = false;
            playerJoint.connectedBody = null;
        }
    }
}