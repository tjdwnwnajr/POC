using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GyroRangeMapping
{
    public float minGyro;      // 구간 시작값
    public float maxGyro;      // 구간 끝값
    public float forceOutput;  // 이 구간에서 출력할 힘
}

public class RopeGrabtest : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private LayerMask ropeLayer;

    [Header("Swing Settings")]
    [SerializeField] private float lessMultiplierX;
    [SerializeField] private float moreMultiplierX;
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
    [Header("Gyro Range Mapping")]
    [SerializeField] private GyroRangeMapping[] gyroRangeMappings = new GyroRangeMapping[]
    {
        new GyroRangeMapping { minGyro = 10f, maxGyro = 100f, forceOutput = 5f },
        new GyroRangeMapping { minGyro = 100f, maxGyro = 200f, forceOutput = 10f },
        new GyroRangeMapping { minGyro = 200f, maxGyro = 300f, forceOutput = 15f },
        new GyroRangeMapping { minGyro = 300f, maxGyro = 400f, forceOutput = 20f },
        new GyroRangeMapping { minGyro = 400f, maxGyro = 400f, forceOutput = 25f }
    };
    private bool wasAbovePositiveOne = false;
    

    [Header("Chain Movement")]
    [SerializeField] private float chainMoveSpeed = 0.1f;
    private bool isMovingChain = false;
    private Rigidbody2D targetChain;
    private Vector3 moveStartPos;
    private Vector3 moveTargetPos;
    private float moveProgress = 0f;
    [SerializeField] private float ropeVelocityThreshold = 0.5f;

    private Rigidbody2D rb;
    private Rigidbody2D swingTarget;
    
    

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
            UpdateChainMovement();
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
        if (PlayerStateList.isRope && !isMovingChain)
        {
            if (InputManager.UpWasPressed)
            {
                MoveToChain(true);
            }
            else if (InputManager.DownWasPressed)
            {
                MoveToChain(false);
            }
        }
        CheckRebound();
    }

   

    void Grab(Rigidbody2D rope)
    {
        
        ropePosX = rope.gameObject.transform.position.x;

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
        Vector2 releaseVelocity = swingTarget.linearVelocity;
        if (releaseVelocity.y < 1f)
        {
            transform.SetParent(null);
            PlayerStateList.isRope = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (releaseVelocity.x > 5f)
        {
            releaseVelocity.x *= lessMultiplierX;
        }
        else
        {
            releaseVelocity.x *= moreMultiplierX;
        }
            
        releaseVelocity.y *= forceMultiplierY;

        releaseVelocity.x = Mathf.Clamp(releaseVelocity.x, -maxVelocityX, maxVelocityX);
        releaseVelocity.y = Mathf.Clamp(releaseVelocity.y, -maxVelocityY, maxVelocityY);
        

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = releaseVelocity;
        //Debug.Log(rb.linearVelocity);
        transform.SetParent(null);
        PlayerStateList.isRope = false;
        
    }
    private void CheckRebound()
    {
        if (PlayerStateList.isGrounded)
        {
            //Debug.Log("착지");
            PlayerStateList.canMove = true;
        }
    }
    
    private void Swing()
    {
        var imu = JSL.JslGetIMUState(0);
        float gyroY = imu.gyroY;
        float absGyroY = Mathf.Abs(gyroY);

        // 노이즈 제거
        if (absGyroY < deadZone)
            return;
        float outputForce = MapGyroToForce(absGyroY);
        float signedForce = outputForce * Mathf.Sign(gyroY);

        // 자이로  수평 힘
        Vector2 forceX = new Vector2(-signedForce, 0f);
        forceX.x = Mathf.Clamp(forceX.x, -maxForce, maxForce);
        
        swingTarget.AddForce(forceX, ForceMode2D.Force);

        float velocityY = swingTarget.linearVelocityY;
        float velocityX = swingTarget.linearVelocityX;
        // 1보다 클 때: 진동
        if (velocityY > 1f)
        {
            if (!wasAbovePositiveOne)  // 방금 1을 넘었을 때만
            {
                if (DualSenseInput.Instance != null)
                {
                    DualSenseInput.Instance.Vibrate(0.0f, 0.3f, 0.3f);
                }
                wasAbovePositiveOne = true;
            }
        }
        else
        {
            wasAbovePositiveOne = false;
        }

    }
   

    private float MapGyroToForce(float absoluteGyroValue)
    {
        // 각 구간을 순회하며 매칭되는 구간 찾기
        foreach (GyroRangeMapping mapping in gyroRangeMappings)
        {
            if (absoluteGyroValue >= mapping.minGyro && absoluteGyroValue < mapping.maxGyro)
            {
                return mapping.forceOutput;
            }
        }

        // 모든 구간을 초과한 경우 마지막 구간의 힘 반환
        if (gyroRangeMappings.Length > 0)
        {
            return gyroRangeMappings[gyroRangeMappings.Length - 1].forceOutput;
        }

        return 0f;
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
    private void MoveToChain(bool moveUp)
    {

        if (Mathf.Abs(swingTarget.linearVelocityX) > ropeVelocityThreshold)
        {
            Debug.Log("밧줄이 움직이는 중. 속도: " + swingTarget.linearVelocityX);
            return;
        }
            Rigidbody2D nextChain = null;
        Transform currentParent = swingTarget.transform.parent;

        if (currentParent == null) return;

        int currentIndex = swingTarget.transform.GetSiblingIndex(); 

        if (moveUp)
        {
            int nextIndex = currentIndex - 1;
            if (nextIndex >= 3)
            {
                Transform siblingTransform = currentParent.GetChild(nextIndex);
                nextChain = siblingTransform.GetComponent<Rigidbody2D>();
            }
        }
        else
        {
            int nextIndex = currentIndex + 1;
            if (nextIndex < currentParent.childCount-1)
            {
                Transform siblingTransform = currentParent.GetChild(nextIndex);
                nextChain = siblingTransform.GetComponent<Rigidbody2D>();
            }
        }

        if (nextChain == null) return;

        isMovingChain = true;
        moveStartPos = transform.position;
        moveTargetPos = nextChain.transform.position;
        //moveTargetPos.y = hand.handPos.y;
        moveProgress = 0f;
        targetChain = nextChain;
    }
    private void UpdateChainMovement()
    {
        if (!isMovingChain)
            return;

        moveProgress += Time.fixedDeltaTime * chainMoveSpeed;

        if (moveProgress >= 1f)
        {
            moveProgress = 1f;
            transform.position = moveTargetPos;
            transform.SetParent(targetChain.transform);
            swingTarget = targetChain;

            if (PlayerStateList.lookingRight)
            {
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }

            isMovingChain = false;
            Debug.Log("체인 이동 완료!");
        }
        else
        {
            transform.position = Vector3.Lerp(moveStartPos, moveTargetPos, moveProgress);
        }
    }
}
