using UnityEngine;
using UnityEngine.UI;
public class SceneBrightManager : MonoBehaviour
{
    public static SceneBrightManager instance;

    [SerializeField] public Image _brightOutImage;

    [Range(0.1f, 10f), SerializeField] private float _brightOutSpeed = 5f;
    [Range(0.1f, 10f), SerializeField] private float _brightInSpeed = 5f;

    [SerializeField] private Color _brightOutStartColor;
    
    public bool IsBrightOut { get; private set; }

    public bool IsBrightIn { get; private set; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }

        _brightOutStartColor.a = 0f;
    }

    private void Update()
    {
        if (IsBrightOut)
        {

            if (_brightOutImage.color.a < 1f)
            {
                _brightOutStartColor.a += Time.deltaTime * _brightOutSpeed;
                _brightOutImage.color = _brightOutStartColor;
            }
            else
            {
                IsBrightOut = false;
            }
        }


        if (IsBrightIn)
        {

            float delta = Time.deltaTime;
            delta = Mathf.Min(delta, 0.016f);

            if (_brightOutImage.color.a > 0f)
            {
                _brightOutStartColor.a -= delta * _brightInSpeed;
                _brightOutImage.color = _brightOutStartColor;
            }
            else
            {
                IsBrightIn = false;
            }
        }
    }
    public void StartBrightOut()
    {
        _brightOutImage.color = _brightOutStartColor;
        IsBrightOut = true;
    }
    public void StartBrightIn()
    {

        if (_brightOutImage.color.a >= 1f)
        {
            _brightOutImage.color = _brightOutStartColor;
            IsBrightIn = true;
        }
    }

   
}
