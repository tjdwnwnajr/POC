using UnityEngine;

public class DebrisProjectile : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] private float lifeTime = 4f;

    [Header("Hit")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool addUpRecoil = false; // true면 위로도 살짝 튕김

    private void OnEnable()
    {
        Invoke(nameof(DestroySelf), lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // PlayerController의 recoil 시스템 사용
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.TriggerRecoilX(); // 바라보는 방향 반대로 X 넉백

            if (addUpRecoil)
                pc.TriggerRecoilYUp(); // (선택) 위로도 살짝
        }

        DestroySelf();
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
