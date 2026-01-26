using UnityEngine;

public class GyroValueMonitor : MonoBehaviour
{
    [SerializeField] private bool logToConsole = true;
    [SerializeField] private bool displayOnScreen = true;
    [SerializeField] private float updateInterval = 0.1f;

    private float gyroMaxX = float.MinValue;
    private float gyroMinX = float.MaxValue;
    private float gyroMaxY = float.MinValue;
    private float gyroMinY = float.MaxValue;
    private float gyroMaxZ = float.MinValue;
    private float gyroMinZ = float.MaxValue;

    private float timer = 0f;

    private void Update()
    {
        var imu = JSL.JslGetIMUState(0);
        float gyroX = imu.gyroX;
        float gyroY = imu.gyroY;
        float gyroZ = imu.gyroZ;

        // 최대값 갱신
        if (gyroX > gyroMaxX) gyroMaxX = gyroX;
        if (gyroY > gyroMaxY) gyroMaxY = gyroY;
        if (gyroZ > gyroMaxZ) gyroMaxZ = gyroZ;

        // 최소값 갱신
        if (gyroX < gyroMinX) gyroMinX = gyroX;
        if (gyroY < gyroMinY) gyroMinY = gyroY;
        if (gyroZ < gyroMinZ) gyroMinZ = gyroZ;

        // 주기적으로 로그
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;

            if (logToConsole)
            {
                Debug.Log($"=== 현재 자이로 값 ===\n" +
                    $"GyroX: {gyroX:F2}\n" +
                    $"GyroY: {gyroY:F2}\n" +
                    $"GyroZ: {gyroZ:F2}\n" +
                    $"\n=== 최대값 ===\n" +
                    $"MaxX: {gyroMaxX:F2}\n" +
                    $"MaxY: {gyroMaxY:F2}\n" +
                    $"MaxZ: {gyroMaxZ:F2}\n" +
                    $"\n=== 최소값 ===\n" +
                    $"MinX: {gyroMinX:F2}\n" +
                    $"MinY: {gyroMinY:F2}\n" +
                    $"MinZ: {gyroMinZ:F2}");
            }
        }
    }

    private void OnGUI()
    {
        if (!displayOnScreen) return;

        GUI.Box(new Rect(10, 10, 300, 250), "Gyro Monitor");

        string text = $"현재값\n" +
            $"X: {JSL.JslGetIMUState(0).gyroX:F2}\n" +
            $"Y: {JSL.JslGetIMUState(0).gyroY:F2}\n" +
            $"Z: {JSL.JslGetIMUState(0).gyroZ:F2}\n\n" +
            $"최대값\n" +
            $"MaxX: {gyroMaxX:F2}\n" +
            $"MaxY: {gyroMaxY:F2}\n" +
            $"MaxZ: {gyroMaxZ:F2}\n\n" +
            $"최소값\n" +
            $"MinX: {gyroMinX:F2}\n" +
            $"MinY: {gyroMinY:F2}\n" +
            $"MinZ: {gyroMinZ:F2}";

        GUI.Label(new Rect(20, 35, 280, 220), text);

        // 초기화 버튼
        if (GUI.Button(new Rect(10, 270, 100, 30), "초기화"))
        {
            ResetValues();
        }
    }

    private void ResetValues()
    {
        gyroMaxX = float.MinValue;
        gyroMinX = float.MaxValue;
        gyroMaxY = float.MinValue;
        gyroMinY = float.MaxValue;
        gyroMaxZ = float.MinValue;
        gyroMinZ = float.MaxValue;

        Debug.Log("자이로 값 초기화 완료");
    }
}