using System;
using System.Collections;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private FMODUnity.StudioEventEmitter _fmod;
    private IEnumerator _currentRoutine;
    public float Delay;

    public bool _isPlaying = false;
    
    public bool Disposable;
    public bool Light;

    [ContextMenu("PlaySound")]
    public void PlaySound()
    {
        if (_fmod == null) return;
        if(_currentRoutine != null) return;
        StartCoroutine(PlaySoundWithDelay());
        _currentRoutine = PlaySoundWithDelay();
    }

    public void StopSound()
    {
        _fmod.Stop();
        _isPlaying = false;
        _currentRoutine = null;
    }

    public bool IsPlaying()
    {
        return _fmod.IsPlaying();
    }

    private IEnumerator PlaySoundWithDelay()
    {
        yield return new WaitForSeconds(Delay);
        _fmod.Play();
        _isPlaying = true;
        if (Disposable)
        {
            _fmod = null;
            _isPlaying = false;
        }

        _currentRoutine = null;
    }
    
    private void OnDestroy()
    {
        if(_fmod != null) _fmod.Stop();
    }
    
    private void OnEnable()
    {
        if (!Light || _isPlaying) return;
        PlaySound();
    }

    private void OnDisable()
    {
        if (!Light) return;
        StopSound();
    }
}
