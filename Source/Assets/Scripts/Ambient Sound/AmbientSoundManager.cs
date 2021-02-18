using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    [SerializeField] private AudioClipUserConfig daytimeEffect = new AudioClipUserConfig("Daytime");
    [SerializeField] private AudioClipUserConfig nightimeEffect = new AudioClipUserConfig("Nighttime");
    [SerializeField] private float transitionTimeSpeedMultiplier = 0.25f;

    float transitionProgress;
    private AudioClipUserConfig currentEffect;    
    private AudioClipUserConfig effectTrainsitioningInto;    
    private AudioClipUserConfig targetEffect;

    SwitchableAudioSources switchableAudioSources;

    void Start()
    {
        currentEffect = daytimeEffect;
        targetEffect = currentEffect;

        switchableAudioSources = new SwitchableAudioSources(GetComponents<AudioSource>()[0], GetComponents<AudioSource>()[1]);
        switchableAudioSources.GetAudioSourceNotPlayingClip(currentEffect.audioClip).clip = currentEffect.audioClip;
        switchableAudioSources.GetAudioSourcePlayingClip(currentEffect.audioClip).volume = currentEffect.volumeMultiplier;
        switchableAudioSources.GetAudioSourcePlayingClip(currentEffect.audioClip).Play();
    }

    void Update()
    {
        if (effectTrainsitioningInto == null && currentEffect != targetEffect)
        {
            effectTrainsitioningInto = targetEffect;
            switchableAudioSources.GetAudioSourceNotPlayingClip(currentEffect.audioClip).clip = effectTrainsitioningInto.audioClip;
            switchableAudioSources.GetAudioSourcePlayingClip(effectTrainsitioningInto.audioClip).volume = 0f;
            switchableAudioSources.GetAudioSourcePlayingClip(effectTrainsitioningInto.audioClip).Play();
            transitionProgress = 0f;
        }

        if (effectTrainsitioningInto != null)
        {
            // Crossfade
            switchableAudioSources.GetAudioSourcePlayingClip(currentEffect.audioClip).volume = currentEffect.volumeMultiplier - (transitionProgress * currentEffect.volumeMultiplier);
            switchableAudioSources.GetAudioSourcePlayingClip(effectTrainsitioningInto.audioClip).volume = transitionProgress * effectTrainsitioningInto.volumeMultiplier;

            transitionProgress += Time.deltaTime * transitionTimeSpeedMultiplier;
            transitionProgress = Mathf.Clamp01(transitionProgress);
            if (transitionProgress >= 1f)
            {
                currentEffect = effectTrainsitioningInto;
                effectTrainsitioningInto = null;

                switchableAudioSources.GetAudioSourcePlayingClip(currentEffect.audioClip).volume = currentEffect.volumeMultiplier;
                switchableAudioSources.GetAudioSourceNotPlayingClip(currentEffect.audioClip).Stop();
                switchableAudioSources.GetAudioSourceNotPlayingClip(currentEffect.audioClip).clip = null;
            }
        }

    }
    void OnTimeChanged(CurrentTime currentTime)
    {
        switch(currentTime)
        {
            case CurrentTime.Day:
                targetEffect = daytimeEffect;
                break;
            case CurrentTime.Night:
                targetEffect = nightimeEffect;
                break;
        }

    }
}
