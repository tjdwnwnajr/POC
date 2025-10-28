using System.Collections;
using UnityEngine;

public class test : MonoBehaviour
{

    Rigidbody2D rb;

    private float xAxis;
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private Transform checkPoint;
    [SerializeField] private float checkDistY = 0.3f;
    [SerializeField] private float checkDistX = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpPower = 45;

    [SerializeField] private float jumpBufferCount = 0;
    [SerializeField] private float jumpBufferFrame = 8;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float coyoteTimeCounter;
    [SerializeField] private int maxJump = 2;
    private int jumpCount = 0;


    [Header("Dash")]
    [SerializeField] private float dashSpeed = 70;
    [SerializeField] private float dashTime=0.35f;
    [SerializeField] private float dashCooldown=0.5f;
    private bool canDash = true;
    private bool dashed=false;
    private float gravity;
    [SerializeField] GameObject dashEffect;


    PlayerStateList pState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        pState = GetComponent<PlayerStateList>();
        gravity = rb.gravityScale;
    }
    private void FixedUpdate()
    {
        GetDirection();
        if (pState.dashing) return;
        MoveX();
    }
    // Update is called once per frame
    void Update()
    {
        UpdateJumpVariables();
        Jump();
        StartDash();

    }
    //
    bool Grounded()
    {
        if (Physics2D.Raycast(checkPoint.position, Vector2.down, checkDistY, groundLayer))
        {
            return true;
        }
        else { return false; }  
    }

    void Jump()
    {
        if(rb.linearVelocityY >0&& Input.GetButtonUp("Jump"))
        {
            rb.linearVelocityY = 0;
           
        }
        if (!pState.jumping&& jumpBufferCount > 0 && coyoteTimeCounter > 0)
        {   
             rb.linearVelocityY = jumpPower;
             pState.jumping = true;
             jumpCount++;

       
            
        }
        else if (!Grounded() && pState.jumping && jumpCount < maxJump && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocityY = jumpPower;
            jumpCount++;
        }
        
    }
    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            jumpCount = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        } 

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCount = jumpBufferFrame;
        }
        else
        {
            jumpBufferCount--;
        }

    }

    void GetDirection()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        if (xAxis > 0)
        {
            //spr.flipX = xAxis < 0;

            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }
    //이동 함수
    void MoveX()
    {
        rb.linearVelocity = new Vector2(xAxis * walkSpeed, rb.linearVelocityY);

        
    }

    void StartDash()
    {


        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;

        }

        if (Grounded())
        {
            dashed = false;

        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;


    }
}
