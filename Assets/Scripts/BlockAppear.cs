using UnityEngine;

public class BlockAppear : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject block;
    [SerializeField]private RotateBlock RotateBlock;
    private bool isActive = false;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D col;

    void Start()
    {
        block = this.gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        spriteRenderer.enabled = false;
        col.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        isActive = RotateBlock.isComplete;
        spriteRenderer.enabled = isActive;
        col.enabled = isActive;


        
    }
}
