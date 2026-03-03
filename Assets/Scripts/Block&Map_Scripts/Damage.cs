using UnityEngine;

public class Damage : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            
            if (!PlayerStateList.isDead)
            {
                PlayerController.Instance.Die();
            }
        }
    }
}
