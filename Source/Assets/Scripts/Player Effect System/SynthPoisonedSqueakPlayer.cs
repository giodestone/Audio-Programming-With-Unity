using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

/// <summary>
/// Plays a 'subtle' squeak similar to when you hear a loud noise by synthysising the sound. Fades in and out too.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SynthPoisonedSqueakPlayer : MonoBehaviour
{
    enum VolumeDirection
    {
        GoToMax,
        GoToMin
    }

    [SerializeField] private float maxVolume = 0.05f;
    [SerializeField] private float pitch = 1000f;

    // According to the c# language spec simple data types (incl. enums) are atomic for both reads and writes. Here they're denoted volatile too for extra performance (don't think it'll be significant here). C# is too kind.
    volatile VolumeDirection requestedTargetVolumeState = VolumeDirection.GoToMin;
    volatile float requestTransitionTime = 1f;

    VolumeDirection currentVolumeDirection = VolumeDirection.GoToMin;
    bool isTransitioning = false;
    float targetTransitionTimeMultiplier;
    float currentVolume;
    float volumeTransitionProgress = 0f;

    float phase;
    float sampleRate;

    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
    }

    // Runs in a separate thread!
    void OnAudioFilterRead(float[] data, int channels)
    {
        // Make sure these values don't change while comparisons are being done.
        var requestedTargetVolumeState = this.requestedTargetVolumeState; 
        var copyTransitionTime = requestTransitionTime;

        // Check whether should be transitioning. Don't allow transitions when transitioning.
        if (!isTransitioning && currentVolumeDirection != requestedTargetVolumeState)
        {
            currentVolumeDirection = requestedTargetVolumeState;
            targetTransitionTimeMultiplier = 1f / copyTransitionTime; // Transition takes a second. This makes it a multiplier. 
            isTransitioning = true;
        }
        
        var increment = pitch * 2f * Mathf.PI / sampleRate;

        for (var i = 0; i < data.Length; i++)
        {
            // Move along where we are on the sine wave.
            phase = phase + increment;
            var secondsPerSample = (sampleRate / data.Length) / 1000f; // Each sample represents some amount of seconds per sample.

            data[i] +=  (currentVolume * Mathf.Sin(phase));
            // Data is same for every channel. Otherwise the data format is interweaving.

            AdvanceCurrentVolume(secondsPerSample); // Need to do for every sample.
        }

    }

    /// <summary>
    /// Changes the current volume based on the <see cref="currentVolumeDirection"/>. Clamped between zero and one.
    /// </summary>
    /// <param name="numToAddToTransitionTime">How much to advance the volume transition</param>
    void AdvanceCurrentVolume(float numToAddToTransitionTime)
    {
        switch (currentVolumeDirection)
        {
            case VolumeDirection.GoToMax:
                volumeTransitionProgress += numToAddToTransitionTime * targetTransitionTimeMultiplier;
                isTransitioning = volumeTransitionProgress < maxVolume;
                break;
            case VolumeDirection.GoToMin:
                volumeTransitionProgress -= numToAddToTransitionTime * targetTransitionTimeMultiplier;
                isTransitioning = volumeTransitionProgress > 0f;
                break;
        }
        volumeTransitionProgress = Mathf.Clamp01(volumeTransitionProgress);
        
        currentVolume = Mathf.Lerp(0, maxVolume, volumeTransitionProgress);

    }

    /// <summary>
    /// Begin playing the sound.
    /// </summary>
    /// <param name="time"></param>
    public void BeginPlayingSound(float time)
    {
        requestTransitionTime = time;
        requestedTargetVolumeState = VolumeDirection.GoToMax;
    }

    /// <summary>
    /// Stop the sound.
    /// </summary>
    /// <param name="time"></param>
    public void StopPlayingSound(float time)
    {
        requestTransitionTime = time;
        requestedTargetVolumeState = VolumeDirection.GoToMin;
    }
}
