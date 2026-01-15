using UnityEngine;
using System.Collections.Generic;

public class RopeGrab : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField] private LayerMask ropeLayer;

    private Rigidbody2D rb;
    private HingeJoint2D ropeJoint;

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
            ReleaseRope();
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
        
    }
    void ReleaseRope()
    {
        if (ropeJoint == null) return;

        Destroy(ropeJoint);
        ropeJoint = null;
        PlayerStateList.isRope = false;
    }
}
