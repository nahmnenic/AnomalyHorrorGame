using System;
using System.Collections;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private FMODUnity.StudioEventEmitter _fmod;
    public float Delay;

    public bool Disposable;

    [ContextMenu("PlaySound")]
    public void PlaySound()
    {
        if (_fmod == null) return;
        StartCoroutine(PlaySoundWithDelay());
    }

    public void StopSound()
    {
        _fmod.Stop();
    }

    public bool IsPlaying()
    {
        return _fmod.IsPlaying();
    }

    private IEnumerator PlaySoundWithDelay()
    {
        yield return new WaitForSeconds(Delay);
        _fmod.Play();
        if (Disposable)
        {
            _fmod = null;
        }
    }
}
