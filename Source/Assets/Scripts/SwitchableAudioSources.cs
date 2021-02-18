using UnityEngine;
using UnityEngine.Assertions;

class SwitchableAudioSources
{
    AudioSource a;
    AudioSource b;

    public SwitchableAudioSources(AudioSource a, AudioSource b)
    {
        this.a = a;
        this.b = b;
    }

    public AudioSource GetAudioSourcePlayingClip(AudioClip clip)
    {        
        if (a.clip == clip)
            return a;
        else if (b.clip == clip)
            return b;

        throw new System.Exception();
    }

    public AudioSource GetAudioSourceNotPlayingClip(AudioClip clip)
    {
        if (a.clip != clip)
            return a;
        else if (b.clip != clip)
            return b;

        throw new System.Exception();
    }
}