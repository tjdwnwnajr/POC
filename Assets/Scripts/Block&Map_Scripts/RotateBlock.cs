using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEditor.PackageManager.UI;

public class RotateBlock : MonoBehaviour
{
    [SerializeField] private ScreenShakeProfile profile;

    bool isActive;
    private CinemachineImpulseSource impulseSource;
    public float rotationAcceleration = 180f; // 입력 시 가속
    public float maxAngularSpeed = 180f;       // 최대 회전 속도
    public float angularDamping = 10f;           // 감속력
    float angularSpeed;
    float input;
    bool inputCheck;
    float checkTimer;
    public float checkRotation;
    public bool isComplete = false;
    float angle;
    void Awake()
    {
        
    }
    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        isActive = false;

    }
    void Update()
    {
        if (!isComplete)
        {
            UpdateInput();
            Rotate();
            CheckCompletion();
        }

        if (isComplete)
        {
            return;
        }
       
    }

    private void UpdateInput()
    {
        if (isActive)
            input = InputManager.Rotate;
        else
            input = 0f;
    }

    private void Rotate()
    {
        if (Mathf.Abs(input) > 0.01f)
        {
            angularSpeed += input * rotationAcceleration * Time.deltaTime;
        }
        else
        {
            // 감속
            angularSpeed = Mathf.Lerp(
                angularSpeed,
                0f,
                angularDamping * Time.deltaTime
            );
        }

        // 최대 속도 제한
        angularSpeed = Mathf.Clamp(
            angularSpeed,
            -maxAngularSpeed,
            maxAngularSpeed
        );

        // 회전 적용
        transform.Rotate(0f, 0f, angularSpeed * Time.deltaTime);
    }
    private void CheckCompletion()
    {
        if (!InputManager.CheckIsHeld)
        {
            checkTimer = 0f;
            return;
        }

        if (CheckRotate(5f))
        {
            checkTimer += Time.deltaTime;

            if (checkTimer >= 2f)
            {
                Complete();
            }
        }
        else
        {
            checkTimer = 0f;
        }
    }
    private void Complete()
    {
        isComplete = true;
        checkTimer = 0f;

        angularSpeed = 0f;

        // 카메라 흔들림 같은 연출
        //CameraShakeManager.instance.CameraShake(cinemachineImpulseSource);
        CameraShakeManager.instance.ScreenShakeFromProfile(profile, impulseSource);

    }


    private bool CheckRotate(float tolerance)
    {
        angle = Mathf.DeltaAngle(0f, transform.eulerAngles.z);

        
        float step = 90f;                // 주기 (예: 90도)
        tolerance = 5f;                  // 허용 오차

        float mod = Mathf.Repeat(angle - checkRotation, step);
        bool isMatch = mod <= tolerance || mod >= step - tolerance;

        return isMatch;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isActive = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isActive = false;
    }
    
}
