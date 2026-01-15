using UnityEngine;
using System.Collections.Generic;

public class RopeSwing : MonoBehaviour
{
    [Header("Rope Detection")]
    [SerializeField] private LayerMask ropeLayer;

    [Header("Swing Settings")]
    [SerializeField] private float swingForce = 5f; // 자동 스윙 힘
    [SerializeField] private float swingForceIncreaseRate = 2f; // 시간당 힘 증가량
    [SerializeField] private float maxSwingForce = 20f; // 최대 스윙 힘
    [SerializeField] private float _swingDirection = 1f; // 1 = 오른쪽, -1 = 왼쪽

    [Header("Release Settings")]
    [SerializeField] private float releaseVelocityMultiplier = 1.3f;
    [SerializeField] private float minReleaseSpeed = 3f;

    [Header("Visual Feedback")]
    [SerializeField] private bool showDebugInfo = true;

    private Rigidbody2D _rb;
    private PlayerMovement _playerMovement;
    private HingeJoint2D _ropeJoint;

    // State
    private bool _isGrabbingRope;
    private Rigidbody2D _grabbedRopeSegment;

    // Swing state
    private float _currentSwingForce;
    private float _swingTimer;
    private float _grabCooldown; // 잡은 직후 바로 놓는 것 방지

    // Trigger detection
    private List<Rigidbody2D> _nearbyRopeSegments = new List<Rigidbody2D>();

    // Original player settings
    private float _originalMass;
    private float _originalGravityScale;
    private float _originalLinearDamping;
    private float _originalAngularDamping;
    private RigidbodyConstraints2D _originalConstraints;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerMovement = GetComponent<PlayerMovement>();

        // 원래 Rigidbody2D 설정 저장
        _originalMass = _rb.mass;
        _originalGravityScale = _rb.gravityScale;
        _originalLinearDamping = _rb.linearDamping;
        _originalAngularDamping = _rb.angularDamping;
        _originalConstraints = _rb.constraints;
    }

    private void Update()
    {
        // 쿨다운 감소
        if (_grabCooldown > 0f)
        {
            _grabCooldown -= Time.deltaTime;
        }

        if (!_isGrabbingRope)
        {
            TryGrabRope();
        }
        else
        {
            UpdateSwing();
            TryReleaseRope();
        }
    }

    private void FixedUpdate()
    {
        if (_isGrabbingRope)
        {
            ApplySwingForce();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 밧줄 레이어인지 확인
        if (((1 << collision.gameObject.layer) & ropeLayer) != 0)
        {
            Rigidbody2D ropeRb = collision.GetComponent<Rigidbody2D>();
            if (ropeRb != null && !_nearbyRopeSegments.Contains(ropeRb))
            {
                _nearbyRopeSegments.Add(ropeRb);

                if (showDebugInfo)
                {
                    Debug.Log($"밧줄 범위 진입: {collision.gameObject.name}");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 밧줄 레이어인지 확인
        if (((1 << collision.gameObject.layer) & ropeLayer) != 0)
        {
            Rigidbody2D ropeRb = collision.GetComponent<Rigidbody2D>();
            if (ropeRb != null && _nearbyRopeSegments.Contains(ropeRb))
            {
                _nearbyRopeSegments.Remove(ropeRb);

                if (showDebugInfo)
                {
                    Debug.Log($"밧줄 범위 이탈: {collision.gameObject.name}");
                }
            }
        }
    }

    private void TryGrabRope()
    {
        // RopeWasPressed 입력으로 밧줄 잡기 시도
        if (InputManager.RopeWasPressed)
        {
            if (_nearbyRopeSegments.Count > 0)
            {
                // 가장 가까운 밧줄 세그먼트 선택
                Rigidbody2D closestRope = FindClosestRope();

                if (closestRope != null)
                {
                    GrabRope(closestRope);
                }
            }
            else if (showDebugInfo)
            {
                Debug.Log("근처에 밧줄이 없습니다!");
            }
        }
    }

    private Rigidbody2D FindClosestRope()
    {
        if (_nearbyRopeSegments.Count == 0) return null;

        Rigidbody2D closest = null;
        float closestDistance = float.MaxValue;

        foreach (Rigidbody2D rope in _nearbyRopeSegments)
        {
            if (rope == null) continue;

            float distance = Vector2.Distance(transform.position, rope.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = rope;
            }
        }

        return closest;
    }

    private void GrabRope(Rigidbody2D ropeSegment)
    {
        _isGrabbingRope = true;
        _grabbedRopeSegment = ropeSegment;

        // 스윙 상태 초기화
        _currentSwingForce = swingForce;
        _swingTimer = 0f;
        _grabCooldown = 0.2f; // 0.2초 동안 놓기 불가

        // 플레이어 위치에 따라 스윙 방향 자동 결정
        Vector2 toRope = ropeSegment.transform.position - transform.position;
        _swingDirection = transform.position.x < ropeSegment.transform.position.x ? 1f : -1f;

        // PlayerMovement 비활성화 (플레이어가 직접 움직일 수 없게)
        if (_playerMovement != null)
        {
            _playerMovement.enabled = false;
        }

        // HingeJoint2D 생성하여 밧줄에 연결
        _ropeJoint = gameObject.AddComponent<HingeJoint2D>();
        _ropeJoint.connectedBody = ropeSegment;
        _ropeJoint.autoConfigureConnectedAnchor = false;

        // Anchor 설정 (플레이어의 중심에서 연결)
        _ropeJoint.anchor = Vector2.zero;
        _ropeJoint.connectedAnchor = Vector2.zero;

        // 조인트 설정
        _ropeJoint.enableCollision = false;

        // 플레이어 물리 설정을 밧줄 스윙에 맞게 변경
        _rb.mass = 1f; // 질량 증가 (스윙 역학을 위해)
        _rb.gravityScale = 2f; // 중력 활성화 (진자 운동)
        _rb.linearDamping = 0.1f; // 적은 공기 저항 (운동량 유지)
        _rb.angularDamping = 0f; // 회전 저항 없음
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Z축 회전만 고정

        // 현재 속도 유지 (점프 중이었다면)
        // _rb.linearVelocity는 그대로 유지

        if (showDebugInfo)
        {
            Debug.Log($"========================================");
            Debug.Log($"밧줄 잡음: {ropeSegment.gameObject.name}");
            Debug.Log($"초기 방향: {(_swingDirection > 0 ? "오른쪽" : "왼쪽")}");
            Debug.Log($"초기 스윙 힘: {_currentSwingForce}");
            Debug.Log($"현재 속도: {_rb.linearVelocity}");
            Debug.Log($"========================================");
        }
    }

    private void UpdateSwing()
    {
        // 시간 경과에 따라 스윙 힘 증가
        _swingTimer += Time.deltaTime;
        _currentSwingForce = Mathf.Min(
            swingForce + (swingForceIncreaseRate * _swingTimer),
            maxSwingForce
        );

        if (showDebugInfo && Time.frameCount % 60 == 0) // 1초마다
        {
            Debug.Log($"[스윙 중] 시간: {_swingTimer:F2}초, 힘: {_currentSwingForce:F1}, 속도: {_rb.linearVelocity.magnitude:F2}");
        }

        // 스윙 방향 전환 감지 (진자처럼 왔다갔다)
        if (_grabbedRopeSegment != null)
        {
            Vector2 velocity = _rb.linearVelocity;

            // 수평 속도가 거의 0이 되면 방향 전환
            if (Mathf.Abs(velocity.x) < 0.5f && velocity.y < 0f)
            {
                _swingDirection *= -1f;

                if (showDebugInfo)
                {
                    Debug.Log($"[방향 전환] → {(_swingDirection > 0 ? "오른쪽" : "왼쪽")}");
                }
            }
        }
    }

    private void ApplySwingForce()
    {
        if (_grabbedRopeSegment == null) return;

        // 밧줄 위치를 기준으로 현재 각도 계산
        Vector2 toPlayer = (Vector2)transform.position - (Vector2)_grabbedRopeSegment.transform.position;
        float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        // 진자 운동을 돕는 수평 방향 힘 추가
        // 각도가 수직에 가까울수록 (최고점) 힘이 강해짐
        float angleFromVertical = Mathf.Abs(angle - 90f);
        float forceFactor = Mathf.Clamp01(angleFromVertical / 90f);

        Vector2 swingForceVector = new Vector2(_swingDirection * _currentSwingForce * forceFactor, 0f);
        _rb.AddForce(swingForceVector, ForceMode2D.Force);

        if (showDebugInfo && Time.frameCount % 30 == 0) // 0.5초마다
        {
            Debug.Log($"[힘 적용] 방향: {(_swingDirection > 0 ? "→" : "←")}, 각도: {angle:F1}°, 힘: {swingForceVector.magnitude:F1}");
        }
    }

    private void TryReleaseRope()
    {
        // 쿨다운이 남아있으면 놓을 수 없음
        if (_grabCooldown > 0f)
        {
            if (showDebugInfo && InputManager.RopeWasPressed)
            {
                Debug.Log($"[쿨다운] 아직 놓을 수 없음 ({_grabCooldown:F2}초 남음)");
            }
            return;
        }

        // RopeWasPressed 다시 누르면 놓기
        if (InputManager.RopeWasPressed)
        {
            ReleaseRope();
        }
    }

    private void ReleaseRope()
    {
        if (!_isGrabbingRope) return;

        // 조인트 제거 전에 현재 속도 먼저 저장
        Vector2 currentVelocity = _rb.linearVelocity;

        if (showDebugInfo)
        {
            Debug.Log($"놓기 전 속도: {currentVelocity}, 크기: {currentVelocity.magnitude:F2}");
        }

        // 조인트 제거 (순서 중요!)
        if (_ropeJoint != null)
        {
            Destroy(_ropeJoint);
            _ropeJoint = null;
        }

        // 속도가 너무 느리면 최소 속도 보장
        if (currentVelocity.magnitude < minReleaseSpeed)
        {
            // 현재 스윙 방향으로 최소 속도 적용
            if (currentVelocity.magnitude > 0.1f)
            {
                currentVelocity = currentVelocity.normalized * minReleaseSpeed;
            }
            else
            {
                // 속도가 거의 0이면 스윙 방향으로
                currentVelocity = new Vector2(_swingDirection * minReleaseSpeed, minReleaseSpeed * 0.5f);
            }
        }

        // 속도 증폭 (축적된 운동량 활용)
        Vector2 releaseVelocity = currentVelocity * releaseVelocityMultiplier;

        // ★★★ 중요: PlayerMovement를 먼저 복원하되 비활성화 상태 유지 ★★★
        _isGrabbingRope = false;
        _grabbedRopeSegment = null;
        _swingTimer = 0f;

        // 플레이어 물리 설정 복원 (아직 속도는 적용하지 않음)
        _rb.mass = _originalMass;
        _rb.gravityScale = _originalGravityScale;
        _rb.linearDamping = _originalLinearDamping;
        _rb.angularDamping = _originalAngularDamping;
        _rb.constraints = _originalConstraints;

        // ★★★ 다음 프레임까지 속도 유지 ★★★
        StartCoroutine(ApplyReleaseVelocity(releaseVelocity));

        if (showDebugInfo)
        {
            Debug.Log($"========================================");
            Debug.Log($"[놓기 전] 속도: {currentVelocity}, 크기: {currentVelocity.magnitude:F2}");
            Debug.Log($"[놓기] 최종 속도: {releaseVelocity}, 크기: {releaseVelocity.magnitude:F2}");
            Debug.Log($"[놓기] 스윙 시간: {_swingTimer:F2}초, 누적 힘: {_currentSwingForce:F2}");
            Debug.Log($"========================================");
        }

        // PlayerMovement를 활성화하기 전에 속도 먼저 설정
        if (_playerMovement != null)
        {
            // SetVerticalVelocity로 상태 먼저 설정
            _playerMovement.SetVerticalVelocity(releaseVelocity.y);
            _playerMovement.enabled = true;
        }

        
    }

    private System.Collections.IEnumerator ApplyReleaseVelocity(Vector2 velocity)
    {
        // 여러 프레임에 걸쳐 속도를 강제로 유지
        for (int i = 0; i < 3; i++)
        {
            _rb.linearVelocity = velocity;
            if (showDebugInfo)
            {
                Debug.Log($"[프레임 {i}] 속도 강제 적용: {velocity}, 현재 속도: {_rb.linearVelocity}");
            }
            yield return new WaitForFixedUpdate();
        }
    }

    // 현재 밧줄을 잡고 있는지 확인 (다른 스크립트에서 사용 가능)
    public bool IsGrabbingRope()
    {
        return _isGrabbingRope;
    }

    // 현재 스윙 힘 확인 (UI 표시용)
    public float GetCurrentSwingForce()
    {
        return _currentSwingForce;
    }

    // 스윙 시간 확인
    public float GetSwingTime()
    {
        return _swingTimer;
    }

    // 강제로 밧줄에서 놓기 (예: 데미지를 받았을 때)
    public void ForceRelease()
    {
        if (_isGrabbingRope)
        {
            ReleaseRope();
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugInfo) return;

        // 근처 밧줄 세그먼트 표시
        if (_nearbyRopeSegments.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach (Rigidbody2D rope in _nearbyRopeSegments)
            {
                if (rope != null)
                {
                    Gizmos.DrawLine(transform.position, rope.transform.position);
                }
            }
        }

        // 현재 잡고 있는 밧줄과의 연결선
        if (_isGrabbingRope && _grabbedRopeSegment != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, _grabbedRopeSegment.transform.position);
            Gizmos.DrawWireSphere(_grabbedRopeSegment.transform.position, 0.3f);

            // 스윙 방향 화살표
            Vector2 forceDir = new Vector2(_swingDirection, 0f);
            Vector3 arrowStart = transform.position;
            Vector3 arrowEnd = arrowStart + (Vector3)forceDir * (_currentSwingForce / maxSwingForce) * 2f;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(arrowStart, arrowEnd);
            Gizmos.DrawWireSphere(arrowEnd, 0.2f);
        }
    }
}