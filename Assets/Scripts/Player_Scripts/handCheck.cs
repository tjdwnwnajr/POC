using System.Collections.Generic;
using UnityEngine;

public class handCheck : MonoBehaviour
{
    public float grabY;
    public float grabX;
    public Vector2 handPos;
    public bool canGrab;
    public Rigidbody2D hangedRope;

    // 접촉 중인 모든 chain들
    private List<Rigidbody2D> touchingRopes = new List<Rigidbody2D>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7) // rope layer
        {
            Rigidbody2D ropeRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ropeRb != null && !touchingRopes.Contains(ropeRb))
            {
                touchingRopes.Add(ropeRb);
                UpdateClosestRope();
                canGrab = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 매 프레임 가장 가까운 rope 업데이트
        if (collision.gameObject.layer == 7)
        {
            UpdateClosestRope();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Rigidbody2D ropeRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ropeRb != null)
            {
                touchingRopes.Remove(ropeRb);
            }

            // 접촉 중인 rope가 남아있으면 업데이트, 없으면 초기화
            if (touchingRopes.Count > 0)
            {
                UpdateClosestRope();
            }
            else
            {
                canGrab = false;
                handPos = Vector2.zero;
                hangedRope = null;
            }
        }
    }

    private void UpdateClosestRope()
    {
        if (touchingRopes.Count == 0)
        {
            hangedRope = null;
            canGrab = false;
            return;
        }

        // 가장 가까운 rope 찾기
        Rigidbody2D closestRope = null;
        float minDistance = float.MaxValue;

        foreach (var rope in touchingRopes)
        {
            if (rope == null) continue;

            float distance = Vector2.Distance(transform.position, rope.position);
            Debug.Log(rope.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestRope = rope;
            }
        }

        // 가장 가까운 rope 설정
        if (closestRope != null)
        {
            hangedRope = closestRope;
            handPos = transform.position;
            canGrab = true;
        }
        else
        {
            hangedRope = null;
            canGrab = false;
        }
    }
}