using System.Collections;
using UnityEngine;

public class BlockAppear : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject block;
    [Header("Appear Block")]
    [SerializeField]private RotateBlock RotateBlock;
    [SerializeField] private GyroRotateBlock missionblock;
    private bool isActive = false;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D col;
    private bool isVibrate = false;

    [SerializeField] private float appearDuration = 1f;

    private void Start()
    {
        block = this.gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        spriteRenderer.enabled = false;
        col.enabled = false;
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (RotateBlock != null)
        {
            isActive = RotateBlock.isComplete;
        }
        else if (missionblock != null)
        {
            isActive = missionblock.isComplete;
        }
        spriteRenderer.enabled = isActive;
        col.enabled = isActive;
        if(isActive&&!isVibrate)
        {
            
            FadeIn();
            DualSenseInput.Instance.Vibrate(0.35f, 0.15f, 0.12f);
            isVibrate = true;
        }
        


    }

    private void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f,1f));
    }
    IEnumerator FadeRoutine(float from, float to)
    {
        
        Color color = spriteRenderer.color;
        float time = 0f;

        color.a = from;
        spriteRenderer.color = color; 

        while (time < appearDuration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(from, to, time);
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = to;
        spriteRenderer.color = color;
        
        
    }
}
