using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotateMap : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Transform mapParent;           // 회전할 대상 (MapParent)
    public Transform centerObject;        // 이 Device의 회전축
    public Transform deviceObject;        // 이 Device 자신

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float liftDuration = 0.5f;
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private float liftHeight = 5f;

    [Header("Objects to Lift")]
    [SerializeField] private List<Rigidbody2D> objectsToLift = new List<Rigidbody2D>();

    [Header("Gyro Settings")]
    [SerializeField] private float gyroDeadZone = 0.2f;
    [SerializeField] private float inputWaitDuration = 3f;
    [SerializeField] private float gyroPositiveThreshold = 0.5f;
    [SerializeField] private float gyroNegativeThreshold = -0.5f;

    private bool isWaitingForInput = false;
    private float inputWaitTimer = 0f;
    private bool isRotating = false;
    private bool canRotate = false;
    private bool rotateEnd = false;

    [Header("Arrow UI")]
    [SerializeField] private GameObject leftArrowPrefab;
    [SerializeField] private GameObject rightArrowPrefab;
    private GameObject leftArrowInstance;
    private GameObject rightArrowInstance;
    [SerializeField] private GameObject leftSelectedPrefab;
    [SerializeField] private GameObject rightSelectedPrefab;
    private GameObject selectedImageInstance;

    private GameObject _player;
    private Rigidbody2D _playerRb;
    private float _originalGravityScale;
    private Animator _playerAnim;
    private Dictionary<Rigidbody2D, float> _objectOriginalGravity = new Dictionary<Rigidbody2D, float>();

    private int currentRotationState = 0;

    // ⭐ Center와 Device를 분리했을 때 저장할 정보
    private Transform centerParent;
    private Transform deviceParent;
    private int centerSiblingIndex;
    private int deviceSiblingIndex;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRb = _player.GetComponent<Rigidbody2D>();
        _originalGravityScale = _playerRb.gravityScale;
        _playerAnim = _player.GetComponent<Animator>();

        foreach (var rb in objectsToLift)
        {
            if (rb != null && !_objectOriginalGravity.ContainsKey(rb))
            {
                _objectOriginalGravity[rb] = rb.gravityScale;
            }
        }

        // mapParent가 설정되지 않으면 부모로 설정
        if (mapParent == null)
        {
            mapParent = transform.parent;
        }

        // deviceObject가 설정되지 않으면 이 스크립트의 gameObject로 설정
        if (deviceObject == null)
        {
            deviceObject = transform;
        }
    }

    void Update()
    {
        Debug.Log(PlayerStateList.isGrounded);
        // 회전 완료 후 플레이어 컨트롤 활성화
        if (!isRotating && PlayerStateList.isGrounded && rotateEnd)
        {
            InputManager.ActivatePlayerControls();
            rotateEnd = false;
        }

        // 입력 대기 중 취소
        if (isWaitingForInput && !isRotating && InputManager.UseToolWasPressed)
        {
            Debug.Log("회전 입력 취소됨");
            isWaitingForInput = false;
            inputWaitTimer = 0f;
            HideArrows();
            PlayerStateList.isView = false;
            return;
        }

        // 회전 입력 시작
        if (!isWaitingForInput && canRotate && InputManager.UseToolWasPressed && !isRotating)
        {
            isWaitingForInput = true;
            inputWaitTimer = 0f;
            _playerAnim.SetBool("isWalk", false);
            _playerAnim.SetBool("isJump", false);
            PlayerStateList.isView = true;
            ShowArrows();
        }

        // 입력 대기 중 자이로 감지
        if (isWaitingForInput && !isRotating)
        {
            inputWaitTimer += Time.deltaTime;

            // 입력 시간 초과
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
                StartCoroutine(RotateMapCoroutine(targetAngle));
                InputManager.DeactivatePlayerControls();
                rotateEnd = false;
                PlayerStateList.isView = false;
                HideArrows();
            }
        }
    }

    /// <summary>
    /// 자이로 입력 확인 및 회전 각도 결정
    /// </summary>
    private bool CheckGyroInput(out float targetAngle)
    {
        targetAngle = 0f;
        var imu = JSL.JslGetIMUState(0);
        float gyroY = imu.gyroY;

        // 데드존 처리
        if (Mathf.Abs(gyroY) < gyroDeadZone)
        {
            return false;
        }

        // 양의 방향 (오른쪽) 회전
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
        // 음의 방향 (왼쪽) 회전
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

    /// <summary>
    /// ⭐ 핵심 함수: Center와 Device를 분리하고 개별 회전
    /// </summary>
    IEnumerator RotateMapCoroutine(float targetAngle)
    {
        isRotating = true;

        StartCoroutine(LiftPlayer());
        LiftObjects();

        yield return new WaitForSeconds(shakeDuration);

        // ⭐ Step 1: Center와 Device를 MapParent에서 분리
        // 나중에 다시 부모로 만들기 위해 정보 저장
        centerParent = centerObject.parent;
        deviceParent = deviceObject.parent;
        centerSiblingIndex = centerObject.GetSiblingIndex();
        deviceSiblingIndex = deviceObject.GetSiblingIndex();

        // MapParent에서 자식 제거 (월드 좌표 유지)
        centerObject.SetParent(null);
        deviceObject.SetParent(null);

        //Debug.Log("Center와 Device가 MapParent에서 분리됨");

        // ⭐ Step 2: 회전에 필요한 초기값 저장
        Vector3 mapParentWorldPos = mapParent.position;
        Quaternion mapParentWorldRot = mapParent.rotation;

        Vector3 centerWorldPos = centerObject.position;    // 이제 월드 좌표 기준
        Quaternion centerWorldRot = centerObject.rotation;

        Vector3 deviceWorldPos = deviceObject.position;
        Quaternion deviceWorldRot = deviceObject.rotation;

        float elapsedTime = 0f;

        // ⭐ Step 3: 회전 진행
        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / rotationDuration;
            float currentAngle = targetAngle * progress;

            // ===== MapParent 회전 (Center를 중심으로) =====
            Vector3 relativeMapParentPos = mapParentWorldPos - centerWorldPos;
            Vector3 rotatedMapParentPos = Quaternion.AngleAxis(currentAngle, Vector3.forward) * relativeMapParentPos;

            mapParent.position = centerWorldPos + rotatedMapParentPos;
            mapParent.rotation = mapParentWorldRot * Quaternion.AngleAxis(currentAngle, Vector3.forward);

            // ===== Center 회전 (Center Pivot을 중심으로) =====
            Quaternion centerRotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
            centerObject.rotation = centerWorldRot * centerRotation;

            // ===== Device 회전 (Center Pivot을 중심으로) =====
            Quaternion deviceRotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
            deviceObject.rotation = deviceWorldRot * deviceRotation;

            yield return null;
        }

        // ⭐ Step 4: 최종 위치 및 회전 정확하게 설정
        Vector3 finalRelativeMapParentPos = mapParentWorldPos - centerWorldPos;
        Vector3 finalRotatedMapParentPos = Quaternion.AngleAxis(targetAngle, Vector3.forward) * finalRelativeMapParentPos;

        mapParent.position = centerWorldPos + finalRotatedMapParentPos;
        mapParent.rotation = mapParentWorldRot * Quaternion.AngleAxis(targetAngle, Vector3.forward);

        centerObject.rotation = centerWorldRot * Quaternion.AngleAxis(targetAngle, Vector3.forward);
        deviceObject.rotation = deviceWorldRot * Quaternion.AngleAxis(targetAngle, Vector3.forward);

        // 회전 상태 업데이트
        if (targetAngle > 0)
        {
            currentRotationState += 1;
        }
        else if (targetAngle < 0)
        {
            currentRotationState -= 1;
        }

        currentRotationState = Mathf.Clamp(currentRotationState, -1, 1);
        HideSelectedImage();

        // ⭐ Step 5: Center와 Device를 다시 MapParent의 자식으로 만들기
        // 원래 위치와 형제 순서 복원
        centerObject.SetParent(centerParent);
        centerObject.SetSiblingIndex(centerSiblingIndex);

        deviceObject.SetParent(deviceParent);
        deviceObject.SetSiblingIndex(deviceSiblingIndex);

        //Debug.Log("Center와 Device가 MapParent에 다시 포함됨");

        _playerAnim.SetBool("isLift", false);
        LowerPlayer();
        LowerObjects();

        isRotating = false;
    }

    /// <summary>
    /// 플레이어를 들어올리는 코루틴
    /// </summary>
    private IEnumerator LiftPlayer()
    {
        // centerObject의 월드 위치를 기준으로 플레이어 들어올리기
        Vector3 targetPosition = new Vector3(
            centerObject.position.x,
            centerObject.position.y + liftHeight,
            _player.transform.position.z
        );
        Vector3 startPosition = _player.transform.position;

        _playerRb.gravityScale = 0f;
        _playerRb.bodyType = RigidbodyType2D.Kinematic;
        _playerRb.linearVelocity = Vector2.zero;

        float elapsedTime = 0f;
        while (elapsedTime < liftDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / liftDuration);

            _player.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        _player.transform.position = targetPosition;
    }

    /// <summary>
    /// 오브젝트들을 들어올리기
    /// </summary>
    private void LiftObjects()
    {
        foreach (var rb in objectsToLift)
        {
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    /// <summary>
    /// 플레이어를 내려놓기
    /// </summary>
    private void LowerPlayer()
    {
        _playerRb.gravityScale = _originalGravityScale;
        _playerRb.bodyType = RigidbodyType2D.Dynamic;
        rotateEnd = true;
    }

    /// <summary>
    /// 오브젝트들을 내려놓기
    /// </summary>
    private void LowerObjects()
    {
        foreach (var rb in objectsToLift)
        {
            if (rb != null && _objectOriginalGravity.ContainsKey(rb))
            {
                rb.gravityScale = _objectOriginalGravity[rb];
            }
        }
    }

    /// <summary>
    /// 방향 화살표 UI 표시
    /// </summary>
    private void ShowArrows()
    {
        if (leftArrowInstance == null && leftArrowPrefab != null)
        {
            leftArrowInstance = Instantiate(leftArrowPrefab, centerObject);
            leftArrowInstance.transform.localPosition = new Vector3(-1f, 1f, 0f);
        }

        if (rightArrowInstance == null && rightArrowPrefab != null)
        {
            rightArrowInstance = Instantiate(rightArrowPrefab, centerObject);
            rightArrowInstance.transform.localPosition = new Vector3(1f, 1f, 0f);
        }
    }

    /// <summary>
    /// 방향 화살표 UI 숨기기
    /// </summary>
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

    /// <summary>
    /// 선택된 방향 이미지 표시
    /// </summary>
    private void ShowSelectedImage(bool isLeft)
    {
        if (selectedImageInstance != null)
        {
            Destroy(selectedImageInstance);
            selectedImageInstance = null;
        }

        if (isLeft && leftSelectedPrefab != null)
        {
            selectedImageInstance = Instantiate(leftSelectedPrefab, centerObject);
            selectedImageInstance.transform.localPosition = new Vector3(-1f, 1f, 0f);
        }
        else if (!isLeft && rightSelectedPrefab != null)
        {
            selectedImageInstance = Instantiate(rightSelectedPrefab, centerObject);
            selectedImageInstance.transform.localPosition = new Vector3(1f, 1f, 0f);
        }
    }

    /// <summary>
    /// 선택된 방향 이미지 숨기기
    /// </summary>
    private void HideSelectedImage()
    {
        if (selectedImageInstance != null)
        {
            Destroy(selectedImageInstance);
            selectedImageInstance = null;
        }
    }

    /// <summary>
    /// 플레이어가 회전 장치 트리거에 진입
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canRotate = true;
        }
    }

    /// <summary>
    /// 플레이어가 회전 장치 트리거에서 탈출
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canRotate = false;
        }
    }
}