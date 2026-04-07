using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class BGMplayandstop : MonoBehaviour
{
    private AudioSource audioSource;
    private bool playOn = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "StartScene")
        {
            PlayON();
        }
    }
    private void PlayON()
    {
        audioSource.Play();
    }
}
