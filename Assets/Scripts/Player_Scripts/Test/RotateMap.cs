using UnityEngine;
using System.Collections;

public class MapRotation : MonoBehaviour
{
    public Transform mapParent;
    public Transform centerObject;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;      // 흔들림 시간

    [Header("Rotation Settings")]
    [SerializeField] private float rotationDuration = 1f;     // 회전 시간

    [Header("Player Lift Settings")]
    [SerializeField] private float liftHeight = 5f;           // 들어올 높이

    private bool isRotating = false;
    private bool canRotate = false;

    private GameObject _player;
    private Rigidbody2D _playerRb;
    private float _originalGravityScale;                      // 원래 중력값 저장

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRb = _player.GetComponent<Rigidbody2D>();
        _originalGravityScale = _playerRb.gravityScale;        // 원래 중력값 저장
    }

    void Update()
    {
        if (canRotate && Input.GetKeyDown(KeyCode.Space) && !isRotating)
        {
            StartCoroutine(RotateMap(-90f));
        }
    }

    IEnumerator RotateMap(float targetAngle)
    {
        isRotating = true;

        // 플레이어 띄우기
        LiftPlayer();

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

        // 플레이어 내리기
        LowerPlayer();

        isRotating = false;
    }

    private void LiftPlayer()
    {
        // 플레이어를 Y방향으로 liftHeight만큼 위로 이동
        _player.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y + 1, _player.transform.position.z);

        // 중력 제거
        _playerRb.gravityScale = 0f;

        // Rigidbody를 Kinematic으로 변경 (Static과 동일한 효과)
        _playerRb.bodyType = RigidbodyType2D.Kinematic;

        // 속도 초기화
        _playerRb.linearVelocity = Vector2.zero;
    }

    private void LowerPlayer()
    {
        // 플레이어를 Y방향으로 liftHeight만큼 아래로 이동
        

        // 중력 복원 (저장했던 원래 값으로)
        _playerRb.gravityScale = _originalGravityScale;

        // Rigidbody를 Dynamic으로 변경
        _playerRb.bodyType = RigidbodyType2D.Dynamic;
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