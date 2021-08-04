using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScriot : MonoBehaviour
{
    public AudioClip buttonSE;
    public AudioSource audioPlayer;

    public void playbuttonSE()
    {
        audioPlayer.PlayOneShot(buttonSE);
    }
}
