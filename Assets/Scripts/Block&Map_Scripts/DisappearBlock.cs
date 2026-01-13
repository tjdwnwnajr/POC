using System.Collections;
using UnityEngine;

public class DisappearBlock : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] float shakeDuration = 3f;
    [SerializeField] float respawnDelay = 3f;

    [Header("Rotation Shake")]
    [SerializeField] float maxRotateAngle = 8f;   // 최대 기울기 (도)
    [SerializeField] float rotateSpeed = 20f;     // 흔들림 속도
    Quaternion startRotation;

    SpriteRenderer sr;
    Collider2D col;

    Vector3 startPos;
    bool triggered;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        startPos = transform.position;
        startRotation = transform.rotation;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        // 모든 접촉 지점 검사
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 위에서 밟았는지 체크
            if (contact.normal.y < -0.5f)
            {
                triggered = true;
                StartCoroutine(PlatformRoutine());
                break;
            }
        }
    }


    IEnumerator PlatformRoutine()
    {
        // 1. 흔들림
        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.deltaTime;

            float angle = Mathf.Sin(Time.time * rotateSpeed) * maxRotateAngle;
            float lift = Mathf.Abs(Mathf.Sin(Time.time * rotateSpeed)) * 0.05f;

            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            transform.position = startPos + Vector3.up * lift;


            yield return null;
        }

        // 2. 사라짐
        DisablePlatform();

        // 3. 재생성 대기
        yield return new WaitForSeconds(respawnDelay);

        transform.rotation = startRotation;
        transform.position = startPos;
        // 4. 복구
        EnablePlatform();
        triggered = false;
    }

    void DisablePlatform()
    {
        sr.enabled = false;
        col.enabled = false;
    }

    void EnablePlatform()
    {
        transform.position = startPos;
        sr.enabled = true;
        col.enabled = true;
    }
}
