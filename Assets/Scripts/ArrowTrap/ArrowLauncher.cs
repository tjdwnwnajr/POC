using System.Collections;
using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private ArrowProjectile arrowPrefab;

    [SerializeField] private float interval = 3f;
    [SerializeField] private bool shootRight = true;

    private Coroutine loop;
    private bool firing;

    public void StartFiring()
    {
        if (firing) return;

        firing = true;
        loop = StartCoroutine(FireLoop());
    }

    public void StopFiring()
    {
        firing = false;

        if (loop != null)
        {
            StopCoroutine(loop);
            loop = null;
        }
    }

    private IEnumerator FireLoop()
    {
        Fire();

        while (firing)
        {
            yield return new WaitForSeconds(interval);
            Fire();
        }
    }

    private void Fire()
    {
        if (firePoint == null || arrowPrefab == null) return;

        Vector2 dir = shootRight ? Vector2.right : Vector2.left;

        ArrowProjectile arrow =
            Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

        arrow.Launch(dir);
    }
}
