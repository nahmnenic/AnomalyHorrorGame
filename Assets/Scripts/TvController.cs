using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TvController : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    
    public void SwitchVideo()
    {
        if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Pause();
            _videoPlayer.gameObject.SetActive(false);
        }
        else
        {
            _videoPlayer.gameObject.SetActive(true);
            _videoPlayer.Play();
        }
    }
}
