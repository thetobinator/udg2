using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class NoTarget_Sound : MonoBehaviour
{
    void PlaySound()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        //audio.PlayDelayed(44100);
    }
    
}