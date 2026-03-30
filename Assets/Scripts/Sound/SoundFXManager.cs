using UnityEngine;
using System.Collections.Generic;

public class SoundFXManager : MonoBehaviour
{
    public enum SFX
    {
        lever=0,
        button=1, 
        wall=2,
        door=3,
        device=4,

    }
    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioClip[] clips;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            
        }
    }

    public void PlaySoundFXClip(SFX audioClip, Transform spawnTransform, float volume, bool useClipLength = true, float settingLength = 1f)
    {
        //spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        if (clips[(int)audioClip] == null)
            return;
        //assign the audio clip
        audioSource.clip = clips[(int)audioClip];

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