using System.Collections;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    [Header("Spawn Range")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    [Header("Prefab")]
    [SerializeField] private DebrisProjectile debrisPrefab;

    [Header("Timing (seconds)")]
    [SerializeField] private float minInterval = 0.6f;
    [SerializeField] private float maxInterval = 1.4f;

    [Header("Trigger")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool spawnOnlyWhilePlayerInside = true;

    private Coroutine loop;
    private bool active;
    //시작하면 debris가 떨어짐
    private void Start()
    {
        StartSpawning();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        StartSpawning();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (spawnOnlyWhilePlayerInside)
            StopSpawning();
    }

    public void StartSpawning()
    {
        if (active) return;
        active = true;
        loop = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        active = false;
        if (loop != null)
        {
            StopCoroutine(loop);
            loop = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (active)
        {
            SpawnOne();
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        }
    }

    private void SpawnOne()
    {
        if (debrisPrefab == null || leftPoint == null || rightPoint == null) return;

        float x = Random.Range(leftPoint.position.x, rightPoint.position.x);
        Vector3 pos = new Vector3(x, leftPoint.position.y, 0f);

        Instantiate(debrisPrefab, pos, Quaternion.identity);
    }
}
