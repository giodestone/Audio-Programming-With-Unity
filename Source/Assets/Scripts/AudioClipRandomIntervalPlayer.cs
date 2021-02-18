using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioClipRandomIntervalPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float minimumTimeBetweenPlaying = 20f;
    [SerializeField] private float maximumTimeBetweenPlaying = 30f;

    private float nextTimeToPlayClip = 0f;

    void Start()
    {
        Assert.IsNotNull(audioSource, $"{nameof(audioSource)} is not set. Set it!");
        Assert.IsNotNull(audioSource.clip, $"{nameof(audioSource)}'s clip is not set. Select a clip for it to play!");
        audioSource.Play();

        Invoke(nameof(PlayClip), 0.1f);
    }

    /// <summary>
    /// Play clip after some time, and invoke its self again.
    /// </summary>
    void PlayClip()
    {
        nextTimeToPlayClip = Time.time + Random.Range(minimumTimeBetweenPlaying, maximumTimeBetweenPlaying);
        audioSource.PlayScheduled(nextTimeToPlayClip);
        nextTimeToPlayClip += audioSource.clip.length; // Next time playing the clip will be considered will be when this clip is over.
        Invoke(nameof(PlayClip), nextTimeToPlayClip);
    }
}
