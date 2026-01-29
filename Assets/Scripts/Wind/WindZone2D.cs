using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WindZone2D : MonoBehaviour
{
    private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var pc = other.GetComponentInParent<PlayerController>();
        if (pc == null) return;

        Bounds b = col.bounds;
        pc.StartWind(this, b.min.y, b.max.y);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var pc = other.GetComponentInParent<PlayerController>();
        if (pc == null) return;

        // 현재 존이 나일 때만 계속 갱신
        // (다른 존과 겹쳐도 값이 튀지 않음)
        Bounds b = col.bounds;
        pc.StartWind(this, b.min.y, b.max.y);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var pc = other.GetComponentInParent<PlayerController>();
        if (pc == null) return;

        pc.EndWind(this);
    }
}
