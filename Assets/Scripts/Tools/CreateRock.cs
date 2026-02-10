using System.Collections;
using UnityEngine;

public class CreateRock : MonoBehaviour
{
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private Vector3 rockSpawnPoint;
    private bool canCreate = false;
    private bool isCreated = false;
    [SerializeField] private float coolTime = 5f;
    private bool endCreate = false;
    private void Start()
    {
        canCreate = false;
    }

    private void Update()
    {
        if(canCreate && InputManager.UseToolWasPressed && !isCreated)
        {
            StartCoroutine(SpawnRock());
        }
    }

    IEnumerator SpawnRock()
    {
        Instantiate(rockPrefab, rockSpawnPoint, Quaternion.identity);
        isCreated = true;
        yield return new WaitForSeconds(coolTime);
        isCreated = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canCreate = true; 
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canCreate = false;
        }
    }
}
