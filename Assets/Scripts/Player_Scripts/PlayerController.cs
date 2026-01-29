using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
//���� ���� ����
// walkSpeed, jumpPower, maxAirjumps, jumpBufferFrames, coyoteTime, dashSpeed, dashTime, dashCooldown, timeBetweenAttack,
// sideAttackArea, upAttackArea, downAttackArea, damage
{   // �̵����� ����



    private Rigidbody2D rb;
    private Animator anim;
    public static PlayerController Instance;
    [HideInInspector] public PlayerStateList pState;

    //cam var
    private float _fallSpeedYDampingChangeThreshold;


    //���� �Ŵ޸���
    [HideInInspector]public bool isRope = false;
    
    
    //movement var
    private Vector2 moveInput;
    [Header("Move Controller")]
    [SerializeField] private float walkSpeed = 1;
    private float xAxis;



    //���� ���� ����
    [Header("Jump Controller")]
    [SerializeField] private float jumpPower = 45; //���� ����
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckDistanceY = 0.2f; //�ٴ� üũ x,y���� 
    [SerializeField] private float groundCheckDistanceX = 0.5f;
    [SerializeField] private LayerMask groundlayer;

    //���� �ּ� ����
    [SerializeField] private float jumpMinTime; //�ּ� ���� �ð�
    private float jumpCountTime = 0; //���� �ð� ���� ����
    private bool jumpCancelImmediate;
    

    //�̴� ����
    private bool jumpwasPressed = false;
    private int airJumpCounter = 0; //���� ���� ī��Ʈ ����
    [SerializeField] private int maxAirJumps; //�ִ� ���� ���� Ƚ��
    
    //���� ���� : ������ �̸� ������ ������ �۵��ϵ��� �ϴ� ����
    private float jumpBufferCounter = 0;
    [SerializeField] private float jumpBufferTime;

    //coyote time : ������ �������� ������ �۵��ϵ��� �ϴº��� 
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;


    //�߷� ����
    [SerializeField] private float fallSpeedLimit = -10f;
    [SerializeField] float gravityUp = 1.2f;        // ��� �� �⺻ �߷�
    [SerializeField] float gravityHang = 0.3f;      // ���� ü�� �߷� (�ٽ�)
    [SerializeField] float gravityFall = 2.0f;      // ���� �� �߷�
    [SerializeField] float hangVelocityThreshold = 0.15f; // ���� ����
    [SerializeField] float gravitySmoothSpeed = 10f;


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


    //////////////////���� ���� 
    
    [SerializeField] private float timeBetweenAttack; //���� �ӵ� ����
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


    [Header("Wind (Buoyancy Style)")]
    public bool inWind;
    [HideInInspector] public float windBottomY;
    [HideInInspector] public float windTopY;

    // 바람 가속(아래 강 / 위 약)
    public float windMaxAccel = 40f;     // 바닥에서의 "위로 가속" (게임 스케일에 맞게)
    public float windPower = 2.0f;       // 1이면 선형, 2~3이면 아래쪽이 훨씬 강해짐

    // 안정화
    public float windVerticalDamping = 6f;  // 수직 속도 감쇠(부력 안정화)
    public float windMaxUpSpeed = 8f;       // 최대 상승 속도 캡
    public float windMinDownSpeedInWind = -20f; // 바람 안에서 낙하가 너무 빨라지는 것 방지(선택)

    // 바람 진입/이탈 부드럽게
    public float windBlendSpeed = 10f;
    private float windBlend;


    private WindZone2D currentWindZone;

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
        //�̵�,�ִϸ��̼��� ���� �ʱ�ȭ
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerStateList>();
       

        PlayerStateList.lookingRight = true;
        pState.invincible = false;
        gravity = rb.gravityScale;
        _fallSpeedYDampingChangeThreshold = CameraManager.Instance._fallSpeedYDampingChangeThresholde;
    }

    private void FixedUpdate()
    {

        //if (pState.dashing) return;
        if (PlayerStateList.canMove&&!PlayerStateList.isView)
        {
            if(xAxis>0|| xAxis < 0)
                TurnCheck();
            MoveX();
            Jump();

            UpdateGravity();       
            LimitFallSpeed();      

            ApplyWindBuoyancy();
            //Recoil();
        }

        // 항상 실행
        if (!PlayerStateList.isView)
        {
            Recoil();
        }

        if (PlayerStateList.isView)
        {
            Debug.Log("����");
            LockPosition();
        }
        else if (!PlayerStateList.isView)
        {
            UnLockPosition();
        }
       


    }
    // Update is called once per frame
    void Update()
    {
        Vector3 center = groundCheckPoint.position;
        Vector3 right = center + new Vector3(groundCheckDistanceX, 0, 0);
        Vector3 left = center + new Vector3(-groundCheckDistanceX, 0, 0);

        // Ray �ð�ȭ
        Debug.DrawRay(center, Vector2.down * groundCheckDistanceY, Color.red);
        Debug.DrawRay(right, Vector2.down * groundCheckDistanceY, Color.red);
        Debug.DrawRay(left, Vector2.down * groundCheckDistanceY, Color.red);
        PlayerStateList.isGrounded = Grounded();
        
        if (PlayerStateList.canMove&&!PlayerStateList.isView)
        {
            GetDirection();
            UpdateJumpVariables();
            //StartDash();

            GetAttack();
            Attack();
            //CatchRope();
        }
        

        //���ϼӵ��� �Ӱ谪���� �۾ƾ���, ī�޶� y�����ӵ��� �̹� �������� ���°� �ƴϾ����, �̹� ���ϻ��·� ������ ���°� �ƴϾ����
        if (rb.linearVelocityY < _fallSpeedYDampingChangeThreshold && !CameraManager.Instance.IsLerpingYDamping && !CameraManager.Instance.LerpedFromPlayerFalling)
        {
            CameraManager.Instance.LerpYDamping(true);
        }
        //�������� �ƴϾ����, ī�޶�y�����ӵ��� ���Ϸ� �����Ǿ��־����,ī�޶� y�����ӵ��� �������� ���°� �ƴϾ����
        if (rb.linearVelocityY >= 0 && CameraManager.Instance.LerpedFromPlayerFalling && !CameraManager.Instance.IsLerpingYDamping)
        {
            //������ ����
            CameraManager.Instance.LerpYDamping(false);
            //���Ϸ� ������ ���°� �ƴ��� �˸��� ���� false�� ����
            CameraManager.Instance.LerpedFromPlayerFalling = false;
        }
    }

  
    //������Լ�
    /////////////////////////////////////////////////////////

    
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

    private void LockPosition()
    {
        
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    private void UnLockPosition()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    private void Turn()
    {
        if (PlayerStateList.lookingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            PlayerStateList.lookingRight = !PlayerStateList.lookingRight;
            
            //_cameraFollowObject.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            PlayerStateList.lookingRight = !PlayerStateList.lookingRight;
           
            //_cameraFollowObject.CallTurn();
        }
    }

    void GetDirection()
    {

        xAxis = InputManager.Movement.x;
        yAxis = InputManager.Movement.y;

    }
    private void TurnCheck()
    {
        if(InputManager.Movement.x>0&&!PlayerStateList.lookingRight)
        {
            Turn();
        }
        else if(InputManager.Movement.x<0&&PlayerStateList.lookingRight)
        {
            Turn();
        }
    }

    //�̵� �Լ�
    void MoveX()
    {
        if (pState.recoilingX) return; // 넉백 중 이동 차단

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

    public bool Grounded() //���̸� true�ƴϸ� false
    {
        //�¿� �� ��, ��� ���� ������ Ȯ��
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistanceY, groundlayer) || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckDistanceX, 0, 0), Vector2.down, groundCheckDistanceY, groundlayer) || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckDistanceX, 0, 0), Vector2.down, groundCheckDistanceY, groundlayer))
        {

            return true;
        }
        return false;
    }

    void UpdateJumpVariables()
    {
        PlayerStateList.isGrounded = Grounded();
        //��������
        if (PlayerStateList.isGrounded)
        {
            jumpCancelImmediate = false;
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        //���� ��
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        //��������
        if (InputManager.JumpWasPressed)
        {
            jumpwasPressed = true;
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        //������
        if (InputManager.JumpWasReleased && pState.jumping && rb.linearVelocityY > 0)
        {
            jumpCancelImmediate = true;
        }

        anim.SetBool("isJump", !PlayerStateList.isGrounded);
    }


    private void Jump()
    {
        jumpCountTime += Time.fixedDeltaTime;
        //���鿡�� ���� ����
        if (!pState.jumping && coyoteTimeCounter > 0 && jumpBufferCounter > 0)

        {
            rb.linearVelocity = new Vector3(rb.linearVelocityX, jumpPower);
            pState.jumping = true;
            jumpCountTime = 0;
            jumpwasPressed = false;
        }
        //�̴� ����
        else if (pState.jumping && jumpwasPressed && airJumpCounter < maxAirJumps)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocityX, jumpPower);
            airJumpCounter++;
            jumpCountTime = 0;
            jumpwasPressed = false;
        }

        //���� ĵ��
        if (jumpCancelImmediate)
        {
            
            if (jumpCountTime >= jumpMinTime)
            {
                jumpCancelImmediate = false;
                if (!inWind)
                    rb.linearVelocityY = 0;
            }
            
        }
    }

    void LimitFallSpeed()
    {
        // Y�ӵ��� fallSpeedLimit���� ������ fallSpeedLimit�� ����
        if (rb.linearVelocityY < fallSpeedLimit)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, fallSpeedLimit);
        }
    }

    

    void UpdateGravity()
    {
        float vy = rb.linearVelocityY;

        // ��ǥ �߷�
        float targetGravity;

        // 1. ��� ��
        if (vy > hangVelocityThreshold)
        {
            targetGravity = gravityUp;
        }
        // 2. ���� ��ó (ü��)
        else if (Mathf.Abs(vy) <= hangVelocityThreshold)
        {
            targetGravity = gravityHang;
        }
        // 3. ���� ��
        else
        {
            targetGravity = gravityFall;
        }

        // �߷� �ε巴�� ��ȯ
        rb.gravityScale = Mathf.Lerp(
            rb.gravityScale,
            targetGravity,
            gravitySmoothSpeed * Time.fixedDeltaTime
        );
    }


    ///////////////////////////////����.....

    void GetAttack()
    {
        pState.attacking = InputManager.AttackWasPressed;
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
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle); //����Ʈ ��Ⱦ ���� ����
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y); //���⿡ �°� �¿� -1�� �ٲ�� ��


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
            if (PlayerStateList.lookingRight)
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
        health = Mathf.Clamp(health, 0, maxhealth);//���� ü��,�ּ�, �ִ�

    }
    IEnumerator StopTakingDamgae()
    {
        pState.invincible = true;
        ClampHealth();
        anim.SetTrigger("takeDamage");
        yield return new WaitForSeconds(1f); //�����ð� ����
        pState.invincible = false;

    }


    public void TriggerRecoilX()
    {
        // 이미 넉백 중이면 새로 갱신하고 싶다면 초기화
        stepsXRecoiled = 0;
        pState.recoilingX = true;
    }

    public void TriggerRecoilYUp()
    {
        // 위로 튕기고 싶을 때(선택)
        stepsYRecoiled = 0;
        yAxis = -1f;              // Recoil()에서 yAxis<0이면 위로 밀어줌
        pState.recoilingY = true;
    }

    //public void UpdateGravityInWind() => rb.gravityScale = Mathf.Lerp(
    //        rb.gravityScale,
    //        windGravity,
    //        windGravitySmooth * Time.fixedDeltaTime
    //    );

    public void StartWind(WindZone2D zone, float bottomY, float topY)
    {
        currentWindZone = zone;
        inWind = true;
        windBottomY = bottomY;
        windTopY = topY;
    }

    public void EndWind(WindZone2D zone)
    {
        // 내가 들어온 존이 나갈 때만 끈다 (겹침/순서 꼬임 방지)
        if (currentWindZone != zone) return;

        inWind = false;
        currentWindZone = null;
    }


    private void ApplyWindBuoyancy()
    {
        float target = inWind ? 1f : 0f;
        windBlend = Mathf.MoveTowards(windBlend, target, Time.fixedDeltaTime * windBlendSpeed);
        if (windBlend <= 0f) return;

        float height = windTopY - windBottomY;
        if (height <= 0.001f) return;

        float t = Mathf.Clamp01((rb.position.y - windBottomY) / height); // 0=바닥, 1=위
        float strength = Mathf.Pow(1f - t, windPower);                   // 아래 강/위 약 (0~1)

        float accelUp = windMaxAccel * strength * windBlend;

        // ✅ 항상 위로 가속 (중력과 경쟁 -> 위에서는 내려오고 아래에서는 다시 붕)
        rb.AddForce(Vector2.up * accelUp * rb.mass, ForceMode2D.Force);

        // ✅ 안전장치: 로켓 방지용 캡 (점프보다 크게!)
        if (rb.linearVelocityY > windMaxUpSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocityX, windMaxUpSpeed);

        // ✅ 낙하가 너무 과하면 하한만 제한(선택)
        if (rb.linearVelocityY < windMinDownSpeedInWind)
            rb.linearVelocity = new Vector2(rb.linearVelocityX, windMinDownSpeedInWind);
    }

}