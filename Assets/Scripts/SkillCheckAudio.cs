using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class SkillCheckAudio : MonoBehaviour
{
    public AudioSource source;
    public AudioClip successClip;
    public AudioClip failClip;

    public void playSuccess()
    {
        source.PlayOneShot(successClip);
    }

    public void playFail()
    {
        source.PlayOneShot(failClip);
    }
}
