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
    [SerializeField] private CinemachineConfiner2D boundary;
    [SerializeField] private CompositeCollider2D defaultBoundary;
    [SerializeField] private CompositeCollider2D rockBoundary;
    private bool endCreate = false;
    private void Start()
    {
        canCreate = false;
        defaultFollowTarget = vcam.Follow;
        boundary.m_BoundingShape2D = defaultBoundary;
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

        boundary.m_BoundingShape2D = rockBoundary;
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
        yield return new WaitForSeconds(5f);
        boundary.m_BoundingShape2D = defaultBoundary;
        vcam.Follow = defaultFollowTarget;
        InputManager.ActivatePlayerControls();
        yield return new WaitForSeconds(coolTime-5f);
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
