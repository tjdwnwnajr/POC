using UnityEngine;

public enum OnDirection
{
    Left,
    Right
}

public class MirrorOnOff : MonoBehaviour
{
    private Collider2D _coll;
    private Vector2 exitDir;
    [SerializeField] private OnDirection OnDir = OnDirection.Left;
    

    private void Awake()
    {
        _coll = GetComponent<Collider2D>();
    }
  
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            exitDir =  (collision.transform.position - _coll.bounds.center).normalized;
            MirrorEffect();
        }
    }
    
    private void MirrorEffect()
    {
        if (OnDir == OnDirection.Left)
        {
            if (exitDir.x < 0f)
                PlayerStateList.isMirror = true;
            else
            {
                PlayerStateList.isMirror = false;
            }
        }
        else
        {
            if (exitDir.x > 0f)
                PlayerStateList.isMirror = true;
            else
            {
                PlayerStateList.isMirror = false;
            }
        }
        
    }
}
