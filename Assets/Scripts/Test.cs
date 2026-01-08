using UnityEngine;

public class RopeGyroForceSwing : MonoBehaviour
{
    public Rigidbody2D swingTarget;   // Chain_01
    public float forceMultiplier = 0.8f;
    public float deadZone = 3f;
    public float maxForce = 20f;

    void FixedUpdate()
    {
        var imu = JSL.JslGetIMUState(0);
        float gyroY = imu.gyroY;

        // 노이즈 제거
        if (Mathf.Abs(gyroY) < deadZone)
            return;

        // 자이로 → 수평 힘
        float forceX = gyroY * forceMultiplier;
        forceX = Mathf.Clamp(forceX, -maxForce, maxForce);

        //  오른쪽 / 왼쪽으로 밀기
        Vector2 force = new Vector2(forceX, 0f);

        swingTarget.AddForce(force, ForceMode2D.Force);
    }
}
