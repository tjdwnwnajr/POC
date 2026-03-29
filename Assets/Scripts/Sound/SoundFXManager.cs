using UnityEngine;
using System.Collections.Generic;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, bool useClipLength = true, float settingLength = 1f)
    {
        //spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audio clip
        audioSource.clip = audioClip;

        //assign the volume
        audioSource.volume = volume;

        //play the audio clip
        audioSource.Play();

        //get the length of the audio clip
        float clipLength;
        if (useClipLength)
            clipLength = audioSource.clip.length;
        else
        {
            clipLength = settingLength;
        }


        //destroy the gameObject after the audio clip has finished playing
        Destroy(audioSource.gameObject, clipLength);

    }

}