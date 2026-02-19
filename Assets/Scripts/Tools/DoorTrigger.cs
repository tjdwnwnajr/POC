using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject doorB;
    [SerializeField] private GameObject keyUsePanel;

    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // 🔥 keyOne이 true일 때만 UI 표시
            if (PlayerStateList.keyOne)
            {
                keyUsePanel.SetActive(true);
            }
            else
            {
                Debug.Log("열쇠 1이 필요합니다.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            keyUsePanel.SetActive(false);
        }
    }

    // ✅ Yes 버튼 연결
    public void OnClickYes()
    {
        if (!playerInRange) return;

        keyUsePanel.SetActive(false);

        // 🔑 열쇠 소모
        PlayerStateList.keyOne = false;

        // doorF 끄기
        gameObject.SetActive(false);

        // doorB 켜기
        if (doorB != null)
        {
            doorB.SetActive(true);
        }
    }

    // ✅ No 버튼 연결
    public void OnClickNo()
    {
        keyUsePanel.SetActive(false);
    }
}
