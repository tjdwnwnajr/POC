using UnityEngine;

public class GyroRotateBlock : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSensitivity = 2f;     // 회전 감도 (높을수록 민감)
    [SerializeField] private float deadZone = 2f;               // 노이즈 제거
    [SerializeField] private float smoothDamping = 0.85f;         // 이전값 반영 (0~1, 작을수록 부드러움)
    [SerializeField] private float maxAngularSpeed = 270f;        // 최대 회전 속도
    [SerializeField] private float maxAngularAcceleration = 160f; // 최대 가속도 제한

    [Header("Completion Settings")]
    [SerializeField] private float checkTolerance = 5f;           // 허용 오차도
    [SerializeField] private float targetAngle = 0f;              // 목표 각도
    [SerializeField] private float holdDuration = 1.5f;           // 목표 각도 유지 시간
    

    private bool isActive = false;
    private float angularSpeed = 0f;                              // 현재 각속도
    private float smoothedGyroY = 0f;                             // 필터링된 자이로값
    private float holdTimer = 0f;
    public bool isComplete = false;
    [SerializeField] private bool noMission = false;
    private void Update()
    {
        if (!isComplete)
        {
            if (isActive)
            {
                UpdateRotationSpeed();
                ApplyRotation();
                CheckCompletion();
            }
        }
    }

    private void UpdateRotationSpeed()
    {
        // 자이로센서 값 가져오기
        var imu = JSL.JslGetIMUState(0);
        float gyroY = imu.gyroY;  // Y축 사용 (위아래 기울임)
        if(PlayerStateList.isMirror)
        {
            gyroY = -gyroY;
        }
        
        // 1. 데드존 처리 (노이즈 제거)
        if (Mathf.Abs(gyroY) < deadZone)
        {
            gyroY = 0f;
        }

        // 2. 저주파 필터 (Exponential Moving Average)
        // 이전값과 현재값을 섞어서 부드럽게 만듦
        smoothedGyroY = Mathf.Lerp(smoothedGyroY, gyroY, (1f - smoothDamping) * Time.deltaTime * 10f);

        // 3. 감도 적용
        float targetSpeed = smoothedGyroY * rotationSensitivity;

        // 4. 최대 속도 제한
        targetSpeed = Mathf.Clamp(targetSpeed, -maxAngularSpeed, maxAngularSpeed);

        // 5. 가속도 제한 적용 (급격한 속도 변화 방지)
        float speedDifference = targetSpeed - angularSpeed;
        float maxAccelerationDelta = maxAngularAcceleration * Time.deltaTime;

        if (Mathf.Abs(speedDifference) > maxAccelerationDelta)
        {
            angularSpeed += Mathf.Sign(speedDifference) * maxAccelerationDelta;
        }
        else
        {
            angularSpeed = targetSpeed;
        }

        //Debug.Log($"GyroY: {gyroY:F2} | Smoothed: {smoothedGyroY:F2} | Speed: {angularSpeed:F2}");
    }

    private void ApplyRotation()
    {
        // Z축 회전 적용
        transform.Rotate(0f, 0f, angularSpeed * Time.deltaTime);
    }

    private void CheckCompletion()
    {
        if(noMission)
        {
            return;
        }
        float diff = Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle);

        // 0도 근처 또는 ±180도 근처면 OK
        // 180도와 -180도를 같다고 봄
        float absDiff = Mathf.Abs(diff);
        if (!InputManager.CheckIsHeld)
        {
            holdTimer = 0f;
            return;
        }
        if (absDiff <= checkTolerance || Mathf.Abs(absDiff - 180f) <= checkTolerance)
        {
            holdTimer += Time.deltaTime;
            DualSenseInput.Instance.Vibrate(0.02f, 0.02f, Time.deltaTime);
            if (holdTimer >= holdDuration) Complete();
        }
        else
        {
            holdTimer = 0f;
        }
    }

    private void Complete()
    {
        isComplete = true;
        holdTimer = 0f;
        angularSpeed = 0f;

        // 목표 각도에 정확히 정렬
        Vector3 currentEuler = transform.eulerAngles;
        currentEuler.z = targetAngle;
        transform.eulerAngles = currentEuler;

        Debug.Log("회전 완료!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isActive = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isActive = false;
        }
    }
}