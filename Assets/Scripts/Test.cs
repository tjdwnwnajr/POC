using UnityEngine;

public class RopeGyroSwing : MonoBehaviour
{
    public Rigidbody2D swingTarget;   // 위쪽 체인 Rigidbody
    public float gyroSensitivity = 0.02f;
    public float maxTorque = 15f;
    public float deadZone = 2f;

    void FixedUpdate()
    {
        // JoyShockLibrary 자이로
        var imu = JSL.JslGetIMUState(0);

        float gyroY = imu.gyroY;

        // 작은 떨림 제거
        if (Mathf.Abs(gyroY) < deadZone)
            return;

        // 자이로 → 토크
        float torque = gyroY * gyroSensitivity;

        // 폭주 방지
        torque = Mathf.Clamp(torque, -maxTorque, maxTorque);

        // 회전력 적용
        swingTarget.AddTorque(-torque, ForceMode2D.Force);
    }
}
