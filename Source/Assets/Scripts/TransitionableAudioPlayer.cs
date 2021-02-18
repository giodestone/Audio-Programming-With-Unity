using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This audio player transitions in and out of audio clips by playing other ones when transitioning out and into.
/// </summary>
public class TransitionableAudioPlayer : MonoBehaviour
{
    /// <summary>
    /// Describes which state <see cref="TransitionableAudioPlayer"/> is in.
    /// </summary>
    public enum TransitionState
    {
        FadingIn,
        MainClip,
        FadingOut,
        Stopped
    }

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip fadeIntoAudioClip;
    [SerializeField] AudioClip mainAudioClip;
    [SerializeField] AudioClip fadeOutAudioClip;
    [SerializeField] TransitionState startingState = TransitionState.Stopped;

    bool isTransitioning = false;
    TransitionState currentState;
    TransitionState transitioningIntoState;
    TransitionState targetState;

    void Start()
    {
        Assert.IsNotNull(audioSource, $"{nameof(audioSource)} to play audio out of has not been set. Set it!");
        Assert.IsNotNull(fadeIntoAudioClip, $"{nameof(fadeIntoAudioClip)} has not been set. Set it!");
        Assert.IsNotNull(mainAudioClip, $"{nameof(mainAudioClip)} has not beed set. Set it!");
        Assert.IsNotNull(fadeOutAudioClip, $"{nameof(fadeOutAudioClip)} has not beed set. Set it!");

        currentState = startingState;
        targetState = currentState;

        audioSource.loop = true;
        audioSource.clip = mainAudioClip;
    }

    void Update()
    {
        if (!isTransitioning && currentState != targetState)
        {
            isTransitioning = true;
            transitioningIntoState = targetState;

            // Would use a switch statement here but Unity's antique C# standard doesn't let me do multi-variable switches.
            if (currentState == TransitionState.Stopped && transitioningIntoState == TransitionState.FadingIn)
            {
                audioSource.PlayOneShot(fadeIntoAudioClip);
                Invoke(nameof(FinishTransitionInto), fadeIntoAudioClip.length);
                
                targetState = TransitionState.MainClip;
            }
            else if (currentState == TransitionState.MainClip && transitioningIntoState == TransitionState.FadingOut)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(fadeOutAudioClip);
                Invoke(nameof(FinishTransitionOut), fadeOutAudioClip.length);
                
                targetState = TransitionState.Stopped;
            }
            else
            {
                // Don't bother with other transitions as they don't make sense.
                isTransitioning = false;
            }
        }
    }

    void FinishTransitionInto()
    {
        audioSource.Play();
        currentState = TransitionState.MainClip;
        isTransitioning = false;
    }

    void FinishTransitionOut()
    {
        currentState = TransitionState.Stopped;
        isTransitioning = false;
    }

    /// <summary>
    /// Signal to begin playing sound.
    /// </summary>
    public void TransitionInto()
    {
        switch (currentState)
        {
            case TransitionState.FadingOut:
                targetState = TransitionState.FadingIn;
                break;
            case TransitionState.Stopped:
                targetState = TransitionState.FadingIn;
                break;
        }
    }

    /// <summary>
    /// Signal to stop sound.
    /// </summary>
    public void TransitionOut()
    {
        switch (currentState)
        {
            case TransitionState.MainClip:
                targetState = TransitionState.FadingOut;
                break;
            case TransitionState.FadingIn:
                targetState = TransitionState.FadingOut;
                break;
        }
    }
}
