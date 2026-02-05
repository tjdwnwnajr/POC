using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapRotation : MonoBehaviour
{

    [Header("Rotation Settings")]
    public Transform mapParent;
    public Transform centerObject;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;      // 흔들림 시간
    [SerializeField] private float liftDuration = 0.5f;       // 들어올리는 시간

    [Header("Rotation Settings")]
    [SerializeField] private float rotationDuration = 1f;     // 회전 시간

    [Header("Player Lift Settings")]
    [SerializeField] private float liftHeight = 5f;           // 들어올 높이

    [Header("Objects to Lift")]
    [SerializeField] private List<Rigidbody2D> objectsToLift = new List<Rigidbody2D>(); // 띄울 오브젝트들

    [Header("Gyro Settings")]
    [SerializeField] private float gyroDeadZone = 0.2f;       // 자이로 데드존
    [SerializeField] private float inputWaitDuration = 3f;    // 입력 대기 시간
    [SerializeField] private float gyroPositiveThreshold = 0.5f; // 자이로 양의 임계값
    [SerializeField] private float gyroNegativeThreshold = -0.5f; // 자이로 음의 임계값
    private bool isWaitingForInput = false;
    private float inputWaitTimer = 0f;

    [Header("Arrow UI")]
    [SerializeField] private GameObject leftArrowPrefab;      // 왼쪽 화살표 prefab
    [SerializeField] private GameObject rightArrowPrefab;     // 오른쪽 화살표 prefab
    private GameObject leftArrowInstance;                     // 왼쪽 화살표 인스턴스
    private GameObject rightArrowInstance;
    [SerializeField] private GameObject leftSelectedPrefab;   // 왼쪽 선택 이미지 prefab
    [SerializeField] private GameObject rightSelectedPrefab;  // 오른쪽 선택 이미지 prefab
    private GameObject selectedImageInstance;

    private bool isRotating = false;
    private bool canRotate = false;

    private GameObject _player;
    private Rigidbody2D _playerRb;
    private float _originalGravityScale;                      // 원래 중력값 저장
    private Animator _playerAnim;

    // 다른 오브젝트들의 원래 중력값 저장
    private Dictionary<Rigidbody2D, float> _objectOriginalGravity = new Dictionary<Rigidbody2D, float>();

    private int currentRotationState = 0;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRb = _player.GetComponent<Rigidbody2D>();
        _originalGravityScale = _playerRb.gravityScale;        // 원래 중력값 저장
        _playerAnim = _player.GetComponent<Animator>();

        // 각 오브젝트의 원래 중력값 저장
        foreach (var rb in objectsToLift)
        {
            if (rb != null && !_objectOriginalGravity.ContainsKey(rb))
            {
                _objectOriginalGravity[rb] = rb.gravityScale;
            }
        }
    }

    void Update()
    {
        // 회전 중이 아닐 때 플레이어 컨트롤 활성화
        if (!isRotating && PlayerStateList.isGrounded)
        {
            InputManager.ActivatePlayerControls();
        }
        // 회전 입력- 취소 입력값을 받고있을 때, 아직 회전은 아닐 때, 다시 누른다면
        if (isWaitingForInput && !isRotating && InputManager.UseToolWasPressed)
        {
            // 취소
            Debug.Log("회전 입력 취소됨");
            isWaitingForInput = false;
            inputWaitTimer = 0f;
            HideArrows();
            PlayerStateList.isView = false;
            return;
        }
        //회전 입력 시작 - 방향 대기상태가 아닐 때, 플레이어가 회전 가능 구역에 있을 때, 회전입력했을 때,회전 중이 아닐 때
        if (!isWaitingForInput&&canRotate && InputManager.UseToolWasPressed && !isRotating) 
        {
            isWaitingForInput = true;
            inputWaitTimer = 0f;
            _playerAnim.SetBool("isWalk",false); // 걷기 애니메이션 중지
            _playerAnim.SetBool("isJump", false);
            PlayerStateList.isView = true;
            ShowArrows();
        }

        // 입력 대기 중
        if (isWaitingForInput && !isRotating)
        {
            inputWaitTimer += Time.deltaTime;

            // 3초 이상 경과하면 취소
            if (inputWaitTimer >= inputWaitDuration)
            {
                Debug.Log("입력 시간 초과 - 회전 취소");
                isWaitingForInput = false;
                inputWaitTimer = 0f;
                HideArrows();
                PlayerStateList.isView = false;
                return;
            }

            // 자이로 입력 확인
            if (CheckGyroInput(out float targetAngle))
            {
                isWaitingForInput = false;
                if (targetAngle == 90f)
                    ShowSelectedImage(true);
                else if (targetAngle == -90f)
                    ShowSelectedImage(false);
                _playerAnim.SetBool("isLift", true);
                StartCoroutine(RotateMap(targetAngle));
                InputManager.DeactivatePlayerControls();
                PlayerStateList.isView = false;
                HideArrows();
                
                
            }
            if (targetAngle == 45f)
            {
                Debug.Log("더 이상 회전할 수 없습니다.");
                isWaitingForInput = false;
                inputWaitTimer = 0f;
                HideArrows();
                PlayerStateList.isView = false;
                return;
            }
        }
    }

    private void ShowArrows()
    {
        // 왼쪽 화살표 생성
        if (leftArrowInstance == null && leftArrowPrefab != null)
        {
            leftArrowInstance = Instantiate(leftArrowPrefab, centerObject);
            leftArrowInstance.transform.localPosition = new Vector3(-1f, 1f, 0f);  // centerObject 왼쪽에 배치
        }

        // 오른쪽 화살표 생성
        if (rightArrowInstance == null && rightArrowPrefab != null)
        {
            rightArrowInstance = Instantiate(rightArrowPrefab, centerObject);
            rightArrowInstance.transform.localPosition = new Vector3(1f, 1f, 0f);  // centerObject 오른쪽에 배치
        }
    }

    // 화살표 UI 숨기기
    private void HideArrows()
    {
        if (leftArrowInstance != null)
        {
            Destroy(leftArrowInstance);
            leftArrowInstance = null;
        }

        if (rightArrowInstance != null)
        {
            Destroy(rightArrowInstance);
            rightArrowInstance = null;
        }
    }

    private void ShowSelectedImage(bool isLeft)
    {
        // 기존 선택 이미지 제거
        if (selectedImageInstance != null)
        {
            Destroy(selectedImageInstance);
            selectedImageInstance = null;
        }

        // 왼쪽 선택
        if (isLeft && leftSelectedPrefab != null)
        {
            selectedImageInstance = Instantiate(leftSelectedPrefab, centerObject);
            selectedImageInstance.transform.localPosition = new Vector3(-1f, 1f, 0f);  // 왼쪽 화살표 위치와 동일
            selectedImageInstance.transform.SetParent(mapParent,true);
        }
        // 오른쪽 선택
        else if (!isLeft && rightSelectedPrefab != null)
        {
            selectedImageInstance = Instantiate(rightSelectedPrefab, centerObject);
            selectedImageInstance.transform.localPosition = new Vector3(1f, 1f, 0f);  // 오른쪽 화살표 위치와 동일
            selectedImageInstance.transform.SetParent(mapParent,true);
        }
    }

    // 선택 이미지 숨기기
    private void HideSelectedImage()
    {
        if (selectedImageInstance != null)
        {
            Destroy(selectedImageInstance);
            selectedImageInstance = null;
        }
    }

    private bool CheckGyroInput(out float targetAngle)
    {
        targetAngle = 0f;
        var imu = JSL.JslGetIMUState(0);
        float gyroY = imu.gyroY;
        
        if (Mathf.Abs(gyroY)< gyroDeadZone )
        {
            // 데드존 내
            return false;
        }
        // 양의 Threshold를 넘으면 90도 회전
        if (gyroY > gyroPositiveThreshold)
        {
            if (currentRotationState + 1 <= 1)
            {
                targetAngle = 90f;
                return true;
            }
            else
            {
                targetAngle = 45f;
                return false;
            }
        }
        // 음의 Threshold를 넘으면 -90도 회전
        else if (gyroY < gyroNegativeThreshold)
        {
            if (currentRotationState - 1 >= -1)
            {
                targetAngle = -90f;
                return true;
            }
            else
            {
                targetAngle = 45f;
                return false;
            }
        }

        return false;
    }
    IEnumerator RotateMap(float targetAngle)
    {
        isRotating = true;

        // 플레이어와 오브젝트들 띄우기
        StartCoroutine(LiftPlayer());
        LiftObjects();

        Vector3 startPosition = mapParent.position;
        Quaternion startRotation = mapParent.rotation;

        // Step 1: 흔들림 (아무것도 안 함, 그냥 대기)
        yield return new WaitForSeconds(shakeDuration);

        // Step 2: 회전 진행
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / rotationDuration; // 0 ~ 1

            // 현재 회전 각도 계산
            float currentAngle = targetAngle * progress;

            // 맵 위치 업데이트 (중심점 기준으로 회전)
            mapParent.position = centerObject.position
                               + Quaternion.AngleAxis(currentAngle, Vector3.forward)
                               * (startPosition - centerObject.position);

            // 맵 회전값 업데이트
            mapParent.rotation = startRotation * Quaternion.AngleAxis(currentAngle, Vector3.forward);

            yield return null;
        }

        // Step 3: 회전 완료 (정확한 값으로 설정)
        mapParent.position = centerObject.position
                           + Quaternion.AngleAxis(targetAngle, Vector3.forward)
                           * (startPosition - centerObject.position);
        mapParent.rotation = startRotation * Quaternion.AngleAxis(targetAngle, Vector3.forward);

        if (targetAngle > 0)
        {
            currentRotationState += 1;  // 오른쪽 회전
        }
        else if (targetAngle < 0)
        {
            currentRotationState -= 1;  // 왼쪽 회전
        }

        // 상태를 -1 ~ 1 범위로 제한
        currentRotationState = Mathf.Clamp(currentRotationState, -1, 1);
        HideSelectedImage();

        // 플레이어와 오브젝트들 내리기
        _playerAnim.SetBool("isLift", false);
        LowerPlayer();
        LowerObjects();

        isRotating = false;
    }

    private IEnumerator LiftPlayer()
    {
        // 목표 위치: centerObject의 X좌표, centerObject의 Y좌표 + 3
        Vector3 targetPosition = new Vector3(centerObject.position.x, centerObject.position.y + 3f, _player.transform.position.z);
        Vector3 startPosition = _player.transform.position;

        // 중력 제거
        _playerRb.gravityScale = 0f;

        // Rigidbody를 Kinematic으로 변경
        _playerRb.bodyType = RigidbodyType2D.Kinematic;

        // 속도 초기화
        _playerRb.linearVelocity = Vector2.zero;
        
        // Lerp로 점진적으로 이동 (shakeDuration 동안)
        float elapsedTime = 0f;
        while (elapsedTime < liftDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / liftDuration); // 0 ~ 1

            _player.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        // 최종 위치 설정
        _player.transform.position = targetPosition;
        
    }

    private void LiftObjects()
    {
        foreach (var rb in objectsToLift)
        {
            if (rb != null)
            {
                // 중력만 제거 (Rigidbody 타입은 변경 안 함)
                rb.gravityScale = 0f;

                // 속도 초기화
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private void LowerPlayer()
    {
        // 중력 복원 (저장했던 원래 값으로)
        _playerRb.gravityScale = _originalGravityScale;

        // Rigidbody를 Dynamic으로 변경
        _playerRb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void LowerObjects()
    {
        foreach (var rb in objectsToLift)
        {
            if (rb != null && _objectOriginalGravity.ContainsKey(rb))
            {
                // 중력 복원 (원래 값으로)
                rb.gravityScale = _objectOriginalGravity[rb];
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canRotate = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canRotate = false;
        }
    }
}