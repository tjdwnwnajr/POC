using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class contactOnOff : MonoBehaviour
{
    [SerializeField] private GameObject gameobject;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private bool On;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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
        Color a = tilemap.color;
        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float per = t / 0.5f;
            a.a = Mathf.Lerp(a.a, 0f, per);

            tilemap.color = a;

            yield return null;
        }

        tilemap.color = new Color(a.r, a.g, a.b, 0f);

    }
    private IEnumerator OnMap()
    {
        Color a = tilemap.color;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            float per = t / 1f;
            a.a = Mathf.Lerp(a.a, 1f, per);

            tilemap.color = a;

            yield return null;
        }

        tilemap.color = new Color(a.r, a.g, a.b, 1f);
    }
}
