using UnityEngine;

public class RopeGyroForceSwing : MonoBehaviour
{
    public Rigidbody2D swingTarget;   // Chain_01
    public float forceMultiplier = 0.8f;
    public float deadZone = 3f;
    public float maxForce = 20f;
    public Vector2 lastVelocity;
    public float lastAngularVelocity;




    void FixedUpdate()
    {
        //var imu = JSL.JslGetIMUState(0);
        //float gyroY = imu.gyroY;

        //// 노이즈 제거
        //if (Mathf.Abs(gyroY) < deadZone)
        //    return;
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
        lastVelocity = swingTarget.linearVelocity;
        lastAngularVelocity = swingTarget.angularVelocity;
        
    }
    private void Update()
    {
        
    }



}
