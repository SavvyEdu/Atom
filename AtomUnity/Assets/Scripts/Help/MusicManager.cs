using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private AudioSource source = null;

    public void SetVolume()
    {
        source.volume = Settings.MUTE ? 0 : 0.3f * Settings.MUSIC_VOLUME;
    }

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
}
