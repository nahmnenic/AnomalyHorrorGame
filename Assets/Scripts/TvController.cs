using System;
using UnityEngine;
using UnityEngine.Video;

public class TvController : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;

    private void Start()
    {
        StopVideo();
    }

    public void StartVideo()
    {
        _videoPlayer.gameObject.SetActive(true);
        _videoPlayer.Play();
    }

    public void StopVideo()
    {
        _videoPlayer.Pause();
        _videoPlayer.gameObject.SetActive(false);
    }
}
