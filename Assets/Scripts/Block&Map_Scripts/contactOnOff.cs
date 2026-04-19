using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class contactOnOff : MonoBehaviour
{
    [SerializeField] private GameObject gameobject;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private bool On;
    private bool isDone = false;
    [SerializeField] private bool shakeOn;
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile profile;

    private void Awake()
    {
        if(shakeOn)
            impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&&!isDone)
        {
            if (gameobject != null)
                gameobject.SetActive(true);
            
            if (tilemap != null)
            {
                if (On)
                {
                    StartCoroutine(OnMap());
                }
                else
                {
                    StartCoroutine(OffMap());
                }
            }
        }
    }
    private IEnumerator OffMap()
    {
        isDone = true;
        Color a = tilemap.color;
        float t = 0;
        while (t < 1.5f)
        {
            t += Time.deltaTime;
            float per = t / 1.5f;
            a.a = Mathf.Lerp(a.a, 0f, per);

            tilemap.color = a;

            yield return null;
        }

        tilemap.color = new Color(a.r, a.g, a.b, 0f);
        
    }
    private IEnumerator OnMap()
    {
        isDone = true;
        Color a = tilemap.color;
        float t = 0;
        if(shakeOn)
            CameraEventManager.instance.CameraShakeEvent(profile, impulseSource);
        while (t < 2.5f)
        {
            t += Time.deltaTime;
            float per = t / 2.5f;
            a.a = Mathf.Lerp(0f, 1f, per);

            tilemap.color = a;

            yield return null;
        }

        tilemap.color = new Color(a.r, a.g, a.b, 1f);
        
    }
}
