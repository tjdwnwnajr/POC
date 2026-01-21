using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGrab : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField] private LayerMask ropeLayer;

    private Rigidbody2D rb;
    private HingeJoint2D ropeJoint;
    [SerializeField] private Vector2 anchorVector;
    [SerializeField] private float forceMultiplier;
    [SerializeField] private float forceMultiplierX;
    [SerializeField] private float forceMultiplierY;
    [SerializeField] private float maxVelocityX;
    [SerializeField] private float maxVelocityY;
    [SerializeField] private float maxForce;
    [SerializeField] private float deadZone;
    private Rigidbody2D swingTarget;
    
    

    // 잡을 수 있는 밧줄 후보들
    private List<Rigidbody2D> nearbyRopes = new List<Rigidbody2D>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

        //if (InputManager.AttackWasPressed)
        //{
        //    StartCoroutine("CheckGyro");
        //}
        
        if (InputManager.RopeIsHeld&&!PlayerStateList.isRope)
        {
            TryGrabRope();
        }
        if(ropeJoint != null && InputManager.RopeWasReleased)
        {
            float finalX = rb.linearVelocityX * forceMultiplierX;
            float finalY = rb.linearVelocityY * forceMultiplierY;
            finalX = Mathf.Clamp(finalX, -maxVelocityX, maxVelocityX);
            finalY = Mathf.Clamp(finalY, -maxVelocityY, maxVelocityY);
            Vector2 finalVelocity = new Vector2(finalX,finalY);

            rb.linearVelocity = finalVelocity;
            ReleaseRope();
        }
        CheckRebound();
        

    }
    IEnumerator CheckGyro()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            var imu = JSL.JslGetIMUState(0);
            float gyroY = imu.gyroY;
            float gyroX = imu.gyroX;
            float gyroZ = imu.gyroZ;
            float accelX = imu.accelX;
            float accelZ = imu.accelZ;
            float accelY = imu.accelY;
            Debug.Log("gyroX: " + gyroX + "gyroY: " + gyroY + "gyroZ: " + gyroZ + "accelX: " + accelX + "accelY: " + accelY + "accelZ: " + accelZ);

        }


    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & ropeLayer) == 0) return;

        Rigidbody2D ropeRb = col.attachedRigidbody;
        if (ropeRb != null && !nearbyRopes.Contains(ropeRb))
        {
            nearbyRopes.Add(ropeRb);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & ropeLayer) == 0) return;

        Rigidbody2D ropeRb = col.attachedRigidbody;
        if (ropeRb != null)
        {
            nearbyRopes.Remove(ropeRb);
        }
    }

    void TryGrabRope()
    {
        if (nearbyRopes.Count == 0) return;

        Rigidbody2D target = FindClosestRope();
        swingTarget = target;
        if (target == null) return;

        
        Grab(target);
    }

    Rigidbody2D FindClosestRope()
    {
        float minDist = float.MaxValue;
        Rigidbody2D closest = null;

        foreach (var rope in nearbyRopes)
        {
            if (rope == null) continue;

            float dist = Vector2.Distance(transform.position, rope.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = rope;
            }
        }

        return closest;
    }

    void Grab(Rigidbody2D rope)
    {
        ropeJoint = gameObject.AddComponent<HingeJoint2D>();
        ropeJoint.connectedBody = rope;
        ropeJoint.autoConfigureConnectedAnchor = false;

        ropeJoint.anchor = anchorVector;
        ropeJoint.connectedAnchor =
        rope.transform.InverseTransformPoint(transform.position);
        PlayerStateList.isRope = true;
        PlayerStateList.canMove = false;
        
    }
    void ReleaseRope()
    {
        if (ropeJoint == null) return;

        Destroy(ropeJoint);
        ropeJoint = null;
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

      
        Debug.Log("Gyro: " + gyroY);
        

        // 자이로  수평 힘
        float forceX = gyroY * forceMultiplier;
        forceX = Mathf.Clamp(forceX, -maxForce, maxForce);

        //  오른쪽 / 왼쪽으로 밀기
        Vector2 force = new Vector2(-forceX, 0f);

        swingTarget.AddForce(force, ForceMode2D.Force);
    }
}
