using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundFXManager : MonoBehaviour
{
    public enum SFX
    {
        lever=0,
        button=1, 
        wall=2,
        door=3,
        device=4,
        walk = 5,
        jump = 6,
        die = 7,
        chest = 8,
        clear = 9,
        portal = 10,
        mirrorOn = 11,
        mirrorOff = 12,
        blockappear = 13,
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
    public void PlaySoundOn(SFX audioSource, float volume)
    {
        StartCoroutine(PlayWalkSound(audioSource,volume));
    }

    IEnumerator PlayWalkSound(SFX audioSource, float volume)
    {
        AudioSource walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.clip = clips[(int)SFX.walk];
        walkAudioSource.volume = volume;
        walkAudioSource.Play();
        Debug.Log("Walk sound started.");
        yield return new WaitForSeconds(walkAudioSource.clip.length * 2); // 발걸음 소리 간격

        Destroy(walkAudioSource);
    }
    

}