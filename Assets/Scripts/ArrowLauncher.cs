using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private ArrowProjectile arrowPrefab;
    [SerializeField] private float speed = 12f;

    // true = 오른쪽, false = 왼쪽
    [SerializeField] private bool shootRight = true;

    private bool fired = false;

    public void Fire()
    {
        if (fired) return;
        fired = true;

        Vector2 dir = shootRight ? Vector2.right : Vector2.left;

        ArrowProjectile arrow =
            Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

        // 방향 회전 (선택)
        float angle = shootRight ? 0f : 180f;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        arrow.Launch(dir, speed);
    }
}
