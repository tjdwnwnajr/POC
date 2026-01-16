using UnityEngine;
using System.Collections.Generic;

public class RopeGrab : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField] private LayerMask ropeLayer;

    private Rigidbody2D rb;
    private HingeJoint2D ropeJoint;

    [SerializeField] private float forceMultiplier;
    [SerializeField] private float forceMultiplierY;
    [SerializeField] private float maxForce;
    private Rigidbody2D swingTarget;

    // 잡을 수 있는 밧줄 후보들
    private List<Rigidbody2D> nearbyRopes = new List<Rigidbody2D>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (InputManager.RopeIsHeld&&!PlayerStateList.isRope)
        {
            TryGrabRope();
        }
        if(ropeJoint != null && InputManager.RopeWasReleased)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * forceMultiplierY);
            ReleaseRope();
        }
        CheckRebound();
        if (PlayerStateList.isRope)
        {
            Swing();
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

        ropeJoint.anchor = new Vector2(0f, 0.8f);
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
        float input = 0f;

        if (Input.GetKey(KeyCode.U))
            input = 1f;
        else if (Input.GetKey(KeyCode.I))
            input = -1f;

        // 입력 없으면 힘 안 줌
        if (Mathf.Abs(input) < 0.01f)
            return;

        // "키보드 gyro"로 사용
        float gyroY = input * 10f; // ← 감도

        // 자이로 → 수평 힘
        float forceX = gyroY * forceMultiplier;
        forceX = Mathf.Clamp(forceX, -maxForce, maxForce);

        //  오른쪽 / 왼쪽으로 밀기
        Vector2 force = new Vector2(-forceX, 0f);

        swingTarget.AddForce(force, ForceMode2D.Force);
    }
}
