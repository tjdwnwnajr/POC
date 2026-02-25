using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RotationDirection
{
    OnlyLeft,   // 왼쪽(-90도)으로만 회전 가능
    OnlyRight,  // 오른쪽(+90도)으로만 회전 가능
    Both,        // 양쪽 모두 회전 가능
    Reverse
}

public class RotateMap : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Transform mapParent;
    [SerializeField] private Transform deviceObject;
    [SerializeField] private RotationDirection rotationDirection = RotationDirection.Both;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float liftDuration = 0.5f;
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private float liftHeight = 3f;

    [Header("Objects to Lift")]
    [SerializeField] private List<Rigidbody2D> objectsToLift = new List<Rigidbody2D>();

    [Header("Gyro Settings")]
    [SerializeField] private float gyroDeadZone = 20f;
    [SerializeField] private float inputWaitDuration = 3f;
    [SerializeField] private float gyroPositiveThreshold = 300f;
    [SerializeField] private float gyroNegativeThreshold = -300f;

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

    // ⭐ Device의 초기 Z 회전 각도 (eulerAngles.z 사용)
    private float deviceInitialZAngle = 0f;

    private Transform deviceParent; //mapparent
    private int deviceSiblingIndex;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRb = _player.GetComponent<Rigidbody2D>();
        _originalGravityScale = _playerRb.gravityScale;
        _playerAnim = _player.GetComponent<Animator>();

        //물체들의 중력 저장
        foreach (Rigidbody2D rb in objectsToLift)
        {
            if (rb != null && !_objectOriginalGravity.ContainsKey(rb))
            {
                _objectOriginalGravity[rb] = rb.gravityScale;
            }
        }


        if (mapParent == null)
        {
            mapParent = transform.parent;
        }

        if (deviceObject == null)
        {
            deviceObject = transform;
        }

        // ⭐ Device의 초기 Z 회전 각도 저장 (eulerAngles.z 사용)
        deviceInitialZAngle = deviceObject.eulerAngles.z;
    }

    void Update()
    {
        // 회전 완료 후 플레이어 컨트롤 활성화
        if (!isRotating && PlayerStateList.isGrounded && rotateEnd)
        {
            InputManager.ActivatePlayerControls();
            rotateEnd = false;
        }

        // 입력 대기 중 취소
        if (isWaitingForInput && !isRotating && InputManager.UseToolWasPressed)
        {
            isWaitingForInput = false;
            inputWaitTimer = 0f;
            HideArrows();
            PlayerStateList.isView = false;
            return;
        }

        // 회전 입력 시작 - ⭐ Device가 사용 가능한 상태인지 확인
        if (!isWaitingForInput && canRotate && InputManager.UseToolWasPressed && !isRotating)
        {
            // ⭐ (MapParent.eulerAngles.z + Device 초기 Z각도) % 360 == 0이면 사용 가능
            if (IsDeviceUsable())
            {
                isWaitingForInput = true;
                inputWaitTimer = 0f;
                _playerAnim.SetBool("isWalk", false);
                _playerAnim.SetBool("isJump", false);
                PlayerStateList.isView = true;
                ShowArrows();
            }
            else
            {
                float mapParentZAngle = mapParent.eulerAngles.z;
                float totalZAngle = mapParentZAngle + deviceInitialZAngle;
                float remainder = totalZAngle % 360f;
                Debug.Log($"Device를 사용할 수 없습니다.\nMapParent Z 각도: {mapParentZAngle}도\nDevice 초기 Z 각도: {deviceInitialZAngle}도\n합: {totalZAngle}도\n합 % 360: {remainder}도");
                return;
            }
        }

        // 입력 대기 중 자이로 감지
        if (isWaitingForInput && !isRotating)
        {
            inputWaitTimer += Time.deltaTime;

            if (inputWaitTimer >= inputWaitDuration)
            {
                isWaitingForInput = false;
                inputWaitTimer = 0f;
                HideArrows();
                PlayerStateList.isView = false;
                return;
            }

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

    
    private bool IsDeviceUsable()
    {
        float mapParentZAngle = mapParent.eulerAngles.z;
        float totalZAngle = mapParentZAngle + deviceInitialZAngle;

        // % 360으로 나머지 계산
        float remainder = totalZAngle % 360f;

        // 부동소수점 오차 고려
        float tolerance = 1f;  // 1도 허용

        return Mathf.Abs(remainder) < tolerance ||
               Mathf.Abs(remainder - 360f) < tolerance;
    }


    private bool CheckGyroInput(out float targetAngle)
    {
        targetAngle = 0f;
        var imu = JSL.JslGetIMUState(0);
        float gyroY = imu.gyroY;

        if (Mathf.Abs(gyroY) < gyroDeadZone)
        {
            return false;
        }
        //positive : 왼쪽
        
        if (gyroY > gyroPositiveThreshold)
        {
            if (rotationDirection == RotationDirection.OnlyRight)
            {
                isWaitingForInput = false;
                inputWaitTimer = 0f;
                HideArrows();
                PlayerStateList.isView = false;
                Debug.Log("이 Device는 오른쪽으로만 회전 가능합니다!");
                return false;
            }
            if (rotationDirection == RotationDirection.Reverse)
            {
                targetAngle = 180f;
            }
            else
            {
                targetAngle = 90f;
            }
            //targetAngle = 90f;
            return true;
        }
        else if (gyroY < gyroNegativeThreshold)
        {

            if (rotationDirection == RotationDirection.OnlyLeft)
            {
                isWaitingForInput = false;
                inputWaitTimer = 0f;
                HideArrows();
                PlayerStateList.isView = false;
                Debug.Log("이 Device는 왼쪽으로만 회전 가능합니다!");
                return false;
            }
            if (rotationDirection == RotationDirection.Reverse)
            {
                targetAngle = 180f;
            }
            else
            {
                targetAngle = 90f;
            }
            //targetAngle = -90f;
            return true;
        }

        return false;
    }

  
    IEnumerator RotateMapCoroutine(float targetAngle)
    {
        isRotating = true;

        StartCoroutine(LiftPlayer());
        LiftObjects();

        yield return new WaitForSeconds(shakeDuration);

        // Step 1: Center와 Device를 MapParent에서 분리
        deviceParent = deviceObject.parent;
        deviceSiblingIndex = deviceObject.GetSiblingIndex();

        deviceObject.SetParent(null);

        // Step 2: 회전에 필요한 초기값 저장
        Vector3 mapParentWorldPos = mapParent.position;
        Quaternion mapParentWorldRot = mapParent.rotation;


        Vector3 deviceWorldPos = deviceObject.position;
        Quaternion deviceWorldRot = deviceObject.rotation;

        float elapsedTime = 0f;

        // Step 3: 회전 진행
        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / rotationDuration;
            float currentAngle = targetAngle * progress;

            // MapParent 회전
            Vector3 relativeMapParentPos = mapParentWorldPos - deviceWorldPos;
            Vector3 rotatedMapParentPos = Quaternion.AngleAxis(currentAngle, Vector3.forward) * relativeMapParentPos;

            mapParent.position = deviceWorldPos + rotatedMapParentPos;
            mapParent.rotation = mapParentWorldRot * Quaternion.AngleAxis(currentAngle, Vector3.forward);


            // Device 회전 (제자리에서만 회전)
            Quaternion deviceRotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
            deviceObject.rotation = deviceWorldRot * deviceRotation;

            yield return null;
        }

        // Step 4: 최종 위치 및 회전 정확하게 설정
        Vector3 finalRelativeMapParentPos = mapParentWorldPos - deviceWorldPos;
        Vector3 finalRotatedMapParentPos = Quaternion.AngleAxis(targetAngle, Vector3.forward) * finalRelativeMapParentPos;

        mapParent.position = deviceWorldPos + finalRotatedMapParentPos;
        mapParent.rotation = mapParentWorldRot * Quaternion.AngleAxis(targetAngle, Vector3.forward);

        deviceObject.rotation = deviceWorldRot * Quaternion.AngleAxis(targetAngle, Vector3.forward);

        //// ⭐ 로그 출력
        //float mapParentZAngle = mapParent.eulerAngles.z;
        //float totalZAngle = mapParentZAngle + deviceInitialZAngle;
        //float remainder = totalZAngle % 360f;
        //Debug.Log($"회전 후:\nMapParent Z 각도: {mapParentZAngle}도\nDevice 초기 Z 각도: {deviceInitialZAngle}도\n합: {totalZAngle}도\n합 % 360: {remainder}도\n사용가능: {IsDeviceUsable()}");

        HideSelectedImage();

        // Step 5: Center와 Device를 다시 MapParent의 자식으로
   

        deviceObject.SetParent(deviceParent);
        deviceObject.SetSiblingIndex(deviceSiblingIndex);

        _playerAnim.SetBool("isLift", false);
        LowerPlayer();
        LowerObjects();

        isRotating = false;
    }

    private IEnumerator LiftPlayer()
    {
        Vector3 targetPosition = new Vector3(
            deviceObject.position.x,
            deviceObject.position.y + liftHeight,
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

    private void LowerPlayer()
    {
        _playerRb.gravityScale = _originalGravityScale;
        _playerRb.bodyType = RigidbodyType2D.Dynamic;
        rotateEnd = true;
    }

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

    private void ShowArrows()
    {
        if (rotationDirection == RotationDirection.OnlyLeft)
        {
            // 왼쪽 화살표만 표시
            if (leftArrowInstance == null && leftArrowPrefab != null)
            {
                leftArrowInstance = Instantiate(leftArrowPrefab, deviceObject);
                leftArrowInstance.transform.localPosition = new Vector3(-1f, 1f, 0f);
            }
        }
        else if (rotationDirection == RotationDirection.OnlyRight)
        {
            // 오른쪽 화살표만 표시
            if (rightArrowInstance == null && rightArrowPrefab != null)
            {
                rightArrowInstance = Instantiate(rightArrowPrefab, deviceObject);
                rightArrowInstance.transform.localPosition = new Vector3(1f, 1f, 0f);
            }
        }

        else // Both
        {
            // 양쪽 화살표 모두 표시
            if (leftArrowInstance == null && leftArrowPrefab != null)
            {
                leftArrowInstance = Instantiate(leftArrowPrefab, deviceObject);
                leftArrowInstance.transform.localPosition = new Vector3(-1f, 1f, 0f);
            }

            if (rightArrowInstance == null && rightArrowPrefab != null)
            {
                rightArrowInstance = Instantiate(rightArrowPrefab, deviceObject);
                rightArrowInstance.transform.localPosition = new Vector3(1f, 1f, 0f);
            }
        } 
    }
        

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
        if (selectedImageInstance != null)
        {
            Destroy(selectedImageInstance);
            selectedImageInstance = null;
        }

        if (isLeft && leftSelectedPrefab != null)
        {
            selectedImageInstance = Instantiate(leftSelectedPrefab, deviceObject);
            selectedImageInstance.transform.localPosition = new Vector3(-1f, 1f, 0f);
        }
        else if (!isLeft && rightSelectedPrefab != null)
        {
            selectedImageInstance = Instantiate(rightSelectedPrefab, deviceObject);
            selectedImageInstance.transform.localPosition = new Vector3(1f, 1f, 0f);
        }
    }

    private void HideSelectedImage()
    {
        if (selectedImageInstance != null)
        {
            Destroy(selectedImageInstance);
            selectedImageInstance = null;
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