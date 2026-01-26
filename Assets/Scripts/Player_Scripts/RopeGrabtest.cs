using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGrabtest : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private LayerMask ropeLayer;
    private bool canGrab;
    

    [Header("Swing Settings")]
    [SerializeField] private float forceMultiplier;
    [SerializeField] private float forceMultiplierX;
    [SerializeField] private float forceMultiplierY;
    [SerializeField] private float maxVelocityX;
    [SerializeField] private float maxVelocityY;
    [SerializeField] private float maxForce;
    [SerializeField] private float deadZone;

    [Header("Hand Settings")]
    public handCheck hand;
    public Vector2 handPos;
    [SerializeField] private float ropePosX;
    [SerializeField] private float ropePosY;



    private Rigidbody2D rb;
    [SerializeField]private Rigidbody2D swingTarget;
    
    

    // 잡을 수 있는 밧줄 후보들
    private List<Rigidbody2D> nearbyRopes = new List<Rigidbody2D>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        if (PlayerStateList.isRope)
        {
            Swing();
        }
    }
    void Update()
    {
        anim.SetBool("isRope",PlayerStateList.isRope);
        if (InputManager.RopeIsHeld&&!PlayerStateList.isRope&&hand.canGrab)
        {
            //TryGrabRope();            
            Grab(hand.hangedRope);
        }
        if (InputManager.RopeWasReleased && PlayerStateList.isRope)
        {
            ReleaseRope();
            StartCoroutine(ResetRotation());
        }
        CheckRebound();
    }

   

    void Grab(Rigidbody2D rope)
    {
        

        ropePosX = rope.gameObject.transform.position.x;
        //handPos = new Vector2(hand.grabX, hand.grabY);

        ropePosY = hand.handPos.y;
        transform.position = new Vector2(ropePosX, ropePosY);
        
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.SetParent(rope.transform);
        float rotateZ = rope.gameObject.transform.rotation.z;
        if (PlayerStateList.lookingRight)
        {
            transform.localRotation = Quaternion.identity;
        }
        else 
        {
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
        swingTarget = rope;

        PlayerStateList.isRope = true;
        PlayerStateList.canMove = false;
        
        
    }
    void ReleaseRope()
    {
        //속도 저장
        Vector2 releaseVelocity = rb.linearVelocity;

        releaseVelocity.x *= forceMultiplierX;
        releaseVelocity.y *= forceMultiplierY;

        releaseVelocity.x = Mathf.Clamp(releaseVelocity.x, -maxVelocityX, maxVelocityX);
        releaseVelocity.y = Mathf.Clamp(releaseVelocity.y, -maxVelocityY, maxVelocityY);
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = releaseVelocity;
        transform.SetParent(null);
        PlayerStateList.isRope = false;
        
    }
    private void CheckRebound()
    {
        if (PlayerStateList.isGrounded)
        {
            Debug.Log("착지");
            PlayerStateList.canMove = true;
        }
    }
    
    private void Swing()
    {
        var imu = JSL.JslGetIMUState(0);
        float gyroY = imu.gyroY;

        // 노이즈 제거
        if (Mathf.Abs(gyroY) < deadZone)
            return;

      
        //Debug.Log("Gyro: " + gyroY);
        

        // 자이로  수평 힘
        float forceX = gyroY * forceMultiplier;
        forceX = Mathf.Clamp(forceX, -maxForce, maxForce);

        //  오른쪽 / 왼쪽으로 밀기
        Vector2 force = new Vector2(-forceX, 0f);
        Transform chainRoot = transform.parent.parent;
        Transform childchain = chainRoot.GetChild(9);
      
        
        swingTarget.AddForce(force, ForceMode2D.Force);
    }
    IEnumerator ResetRotation()
    {
        float duration = 0.2f;
        float t = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.identity; // (0,0,0)
        if (PlayerStateList.lookingRight)
        {
            startRot = transform.rotation;
            targetRot = Quaternion.identity; // (0,0,0)
        }
        else 
        {
            startRot = transform.rotation;
            targetRot = Quaternion.Euler(0f, -180f, 0f);
        }

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
                yield return null;
            }

        transform.rotation = targetRot; // 오차 보정
    }

}
