using System.Collections;
using UnityEngine;


public class CameraTarget : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _flipRotationTime = 0.5f;

    private Coroutine _turnCoroutine;
    private PlayerController _player;
    private bool _isRight;

   
    

    void Awake()
    {
        _player = _playerTransform.GetComponent<PlayerController>();
     
        _isRight = _player.pState.lookingRight;

    }
    private void Update()
    {
        transform.position = _player.transform.position;

    }
    public void CallTurn()
    {
        //_turnCoroutine = StartCoroutine(FlipYLerp());
        //LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipRotationTime).setEaseInOutSine();
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < _flipRotationTime)
        {
            elapsedTime += Time.deltaTime;
            yRotation = Mathf.Lerp(startRotation, endRotationAmount, (elapsedTime / _flipRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        yield return null;
    }

    public float DetermineEndRotation()
    {
        _isRight = !_isRight;
        if (_isRight)
        {
            return 0f;
            
        }
        else
        {
            return 180f;
        }
    }

  
}
