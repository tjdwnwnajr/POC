using UnityEngine;
using UnityEngine.InputSystem;

public class RotateBlock : MonoBehaviour
{
    

    bool isActive;
    private PlayerController playerInput;

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
        playerInput = PlayerController.Instance;
        isActive = false;
    }
    void Update()
    {
       

        inputCheck = playerInput.checkPressed;
        if (!isComplete)
        {
            if (isActive)
            {
                input = playerInput.rotateInput;
            }
            else input = 0f;
            // 1. 입력 → 가속
            if (Mathf.Abs(input) > 0.01f)
            {
                angularSpeed += input * rotationAcceleration * Time.deltaTime;
            }
            else
            {
                // 2. 입력 없으면 감속
                angularSpeed = Mathf.Lerp(
                    angularSpeed,
                    0f,
                    angularDamping * Time.deltaTime
                );
            }

            // 3. 최대 속도 제한
            angularSpeed = Mathf.Clamp(
                angularSpeed,
                -maxAngularSpeed,
                maxAngularSpeed
            );

            // 4. 회전 적용
            transform.Rotate(0f, 0f, angularSpeed * Time.deltaTime);

            
            if (inputCheck)
            {
                if(CheckRotate(5f))
                {
                    checkTimer += Time.deltaTime;

                    if (checkTimer >= 2f)
                    {
                        isComplete = true;
                        
                        
                        
                    }
                }
                else
                {
                    checkTimer = 0f;
                }
            }
        }
        else
        {
            checkTimer = 0f;
        }
    }
    private bool CheckRotate(float tolerance)
    {
        angle = Mathf.DeltaAngle(0f, transform.eulerAngles.z);

        
        float step = 90f;                // 주기 (예: 90도)
        tolerance = 5f;                  // 허용 오차

        float mod = Mathf.Repeat(angle - checkRotation, step);
        Debug.Log("mod: "  + mod);
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
