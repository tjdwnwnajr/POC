using UnityEngine;

[System.Serializable]
public class BackgroundElement
{
    public SpriteRenderer backgroundSprite;
    [Range(0, 1)] public float scrollSpeed;
    [HideInInspector] public Material spriteMaterial;
    [HideInInspector] public Vector2 uvOffset;
}
    public class BackGroundScrolling : MonoBehaviour
{
    public Camera cam;
    private const float SCROLL_SPEED_MULTIPLIER = 0.1f;
    [SerializeField] private BackgroundElement[] backgroundElements;

    private void Start()
    {
        foreach (BackgroundElement element in backgroundElements)
        {
            element.spriteMaterial = element.backgroundSprite.material;
            element.uvOffset = Vector2.zero;
        }
    }
    private void Update()
    {
        foreach (BackgroundElement element in backgroundElements)
        {
            element.uvOffset.x = cam.transform.position.x * element.scrollSpeed * SCROLL_SPEED_MULTIPLIER;
            element.spriteMaterial.SetVector(
                "_UVOffset",
                new Vector4(element.uvOffset.x, element.uvOffset.y, 0, 0)
            );
        }
    }
}
