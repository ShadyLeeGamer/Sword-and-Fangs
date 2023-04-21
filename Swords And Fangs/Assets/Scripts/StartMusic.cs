using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMusic : MonoBehaviour
{
    public AudioClip Music;
    private void Start()
    {
        AudioStation.Instance.StartNewMusicPlayer(Music, true);
    }
}
