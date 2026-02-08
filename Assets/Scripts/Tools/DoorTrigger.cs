using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject doorB;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // doorF 끄기 (이 스크립트가 붙은 오브젝트)
            gameObject.SetActive(false);

            // doorB 켜기
            if (doorB != null)
            {
                doorB.SetActive(true);
            }
        }
    }
}
