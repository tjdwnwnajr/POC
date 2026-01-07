using UnityEngine;

public class RopeSwingForce : MonoBehaviour
{
    public float swingForce = 8f;   // 흔들리는 힘 세기

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // 현재 움직이는 방향 기준으로 좌우 힘
            float dir = Mathf.Sign(rb.linearVelocity.x);

            // 거의 안 움직일 때는 기본 방향
            if (Mathf.Abs(dir) < 0.1f)
                dir = 1f;

            Vector2 force = Vector2.right * dir * swingForce;
            rb.AddForce(force, ForceMode2D.Force);
        }
    }
}
