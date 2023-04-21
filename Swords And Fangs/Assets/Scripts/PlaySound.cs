using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioClip[] Sounds;
    public void Play(int Index)
    {
        AudioStation.Instance.StartNewSFXPlayer(Sounds[Index]);
    }
}
