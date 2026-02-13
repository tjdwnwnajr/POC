using System.Collections;
using UnityEngine;
using Cinemachine;
public class CreateRock : MonoBehaviour
{
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private Vector3 rockSpawnPoint;
    private bool canCreate = false;
    [HideInInspector]public bool isCreated = false;
    [SerializeField] private float coolTime = 5f;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private Transform defaultFollowTarget;

    private bool endCreate = false;
    private void Start()
    {
        canCreate = false;
        defaultFollowTarget = vcam.Follow;
    }

    private void Update()
    {
        if(canCreate && InputManager.UseToolWasPressed && !isCreated)
        {
            //StartCoroutine(SpawnRock());
        }
    }
    public void TrySpawn()
    {
        if (isCreated) return;

        StartCoroutine(SpawnRock());
    }
    IEnumerator SpawnRock()
    {
        InputManager.DeactivatePlayerControls();
        GameObject rock = Instantiate(rockPrefab, rockSpawnPoint, Quaternion.identity);
        isCreated = true;
        if (vcam != null && rock != null)
        {
            vcam.Follow = rock.transform;
        }
        yield return new WaitForSeconds(4f);
        vcam.Follow = defaultFollowTarget;
        InputManager.ActivatePlayerControls();
        yield return new WaitForSeconds(coolTime-4f);
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
