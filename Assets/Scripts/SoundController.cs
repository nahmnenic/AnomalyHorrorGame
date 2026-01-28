using System;
using System.Collections;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private FMODUnity.StudioEventEmitter _fmod;
    public float Delay;

    public void PlaySound()
    {
        StartCoroutine(PlaySoundWithDelay());
    }

    public void StopSound()
    {
        _fmod.Stop();
    }

    private IEnumerator PlaySoundWithDelay()
    {
        yield return new WaitForSeconds(Delay);
        _fmod.Play();
    }
}
