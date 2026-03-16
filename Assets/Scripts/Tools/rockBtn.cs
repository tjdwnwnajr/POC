using UnityEngine;

public class rockBtn : MonoBehaviour
{
    private Animator anim;
    

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rock"))
        {
            anim.SetBool("isTrigger", true);
        }
    }
}
