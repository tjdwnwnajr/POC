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
