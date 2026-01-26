using UnityEngine;

public class GyroMapper : MonoBehaviour
{
    [SerializeField] private float deadZone = 20f;
    [SerializeField] private float smoothing = 0.6f;
    [SerializeField] private float maxGyroValue = 500f; // -500~500

    private float smoothedValue = 0f;

    /// <summary>
    /// 자이로값을 -50~50으로 매핑
    /// -500 = -50, 0 = 0, 500 = 50
    /// </summary>
    public float MapGyroTo50Range(float rawGyroY)
    {
        // 1. 데드존 제거
        if (Mathf.Abs(rawGyroY) < deadZone)
        {
            rawGyroY = 0f;
        }

        // 2. 스무딩
        smoothedValue = Mathf.Lerp(smoothedValue, rawGyroY, smoothing);

        // 3. -500~500을 -50~50으로 변환
        float mapped = Mathf.Clamp(smoothedValue / maxGyroValue * 50f, -50f, 50f);

        return mapped;
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 300, 80), "Gyro Mapper");

        var imu = JSL.JslGetIMUState(0);
        float rawGyroY = imu.gyroY;
        float mapped = MapGyroTo50Range(rawGyroY);

        string text = $"Raw GyroY: {rawGyroY:F2}\n" +
                      $"Mapped (-50~50): {mapped:F2}";

        GUI.Label(new Rect(20, 35, 280, 60), text);
    }
}