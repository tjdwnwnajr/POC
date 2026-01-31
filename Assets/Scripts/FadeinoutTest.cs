using UnityEngine;
using UnityEngine.UI;
public class FadeinoutTest : MonoBehaviour
{
    public static FadeinoutTest instance;

    [SerializeField] public Image _fadeOutImage;
    [Range(0.1f, 10f), SerializeField] private float _fadeOutSpeed = 5f;
    [Range(0.1f, 10f), SerializeField] private float _fadeInSpeed = 5f;

    [SerializeField] private Color _fadeOutStartColor;

    public bool IsFadingOut { get; private set; }

    public bool IsFadingIn { get; private set; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        Debug.Log("æ¿»£√‚");
        _fadeOutStartColor.a = 0f;
    }

    private void Update()
    {
        if (InputManager.AttackWasPressed) { StartFadeOut(); }
        if(InputManager.UseToolWasPressed) { StartFadeIn(); }
        if (IsFadingOut)
        {
            Debug.Log("Fading Out... alpha = " + _fadeOutImage.color.a);
            if (_fadeOutImage.color.a < 1f)
            {
                _fadeOutStartColor.a += Time.deltaTime * _fadeOutSpeed;
                _fadeOutImage.color = _fadeOutStartColor;
            }
            else
            {
                IsFadingOut = false;
            }
        }


        if (IsFadingIn)
        {
            Debug.Log("Fading In... alpha = " + _fadeOutImage.color.a);
            if (_fadeOutImage.color.a > 0f)
            {
                _fadeOutStartColor.a -= Time.deltaTime * _fadeInSpeed;
                _fadeOutImage.color = _fadeOutStartColor;
            }
            else
            {
                IsFadingIn = false;
            }
        }
    }
    public void StartFadeOut()
    {
        _fadeOutImage.color = _fadeOutStartColor;
        IsFadingOut = true;
    }
    public void StartFadeIn()
    {

        if (_fadeOutImage.color.a >= 1f)
        {
            _fadeOutImage.color = _fadeOutStartColor;
            IsFadingIn = true;
        }
    }
}
