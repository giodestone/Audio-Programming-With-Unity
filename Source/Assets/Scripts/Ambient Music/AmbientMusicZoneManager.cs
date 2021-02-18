using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AmbientMusicZoneManager : MonoBehaviour
{
    /// <summary>
    /// For references 
    /// </summary>
    internal class MusicInfo
    {
        public AudioClip audioClip { get; private set; }
        public float volumeMultiplier { get; private set; }
        public float amountPlayed { get; set; }

        public MusicInfo(AudioClip audioClip, float volumeMultiplier)
        {
            this.audioClip = audioClip;
            this.volumeMultiplier = volumeMultiplier;
            amountPlayed = 0f;
        }
    }
    [SerializeField, Tooltip("First Item MUST be the default one.")] private List<AudioClipUserConfig> musicAreaUserConfig;
    private Dictionary<string, MusicInfo> musicAreaConfigs;
    [SerializeField] private float crossfadeTimeMultiplier = 0.5f;
    private SwitchableAudioSources switchableAudioSources;
    bool isTransitioning = false;
    private string currentMusicName;
    private string currentlyTransitioningIntoMusicAreaName;
    private string targetMusicAreaName;
    private float transitionProgress;

    void Start()
    {
        Assert.IsTrue(musicAreaUserConfig.Count > 0);
        ConvertUserConfigIntoInternalDictionary();
        switchableAudioSources = new SwitchableAudioSources(GetComponents<AudioSource>()[0], GetComponents<AudioSource>()[1]);
       
        // Begin playing the main audio area
        currentMusicName = musicAreaUserConfig[0].audioAreaName;
        targetMusicAreaName = currentMusicName;
        switchableAudioSources.GetAudioSourceNotPlayingClip(musicAreaConfigs[currentMusicName].audioClip).clip = musicAreaConfigs[currentMusicName].audioClip;
        switchableAudioSources.GetAudioSourcePlayingClip(musicAreaConfigs[currentMusicName].audioClip).volume = 1f;
        switchableAudioSources.GetAudioSourcePlayingClip(musicAreaConfigs[currentMusicName].audioClip).Play();
        // Don't set where to seek to because it doesn't matter.
    }

    /// <summary>
    /// Convert the user config into a dictionary.
    /// </summary>
    void ConvertUserConfigIntoInternalDictionary()
    {
        musicAreaConfigs = new Dictionary<string, MusicInfo>(musicAreaUserConfig.Count);
        foreach (var mauc in musicAreaUserConfig)
        {
            Assert.IsTrue(mauc.audioAreaName != "");
            Assert.IsNotNull(mauc.audioClip);
            musicAreaConfigs.Add(mauc.audioAreaName, new MusicInfo(mauc.audioClip, mauc.volumeMultiplier));
        }
    }

    void Update()
    {
        // Check if working on transition.
        if (!isTransitioning && currentMusicName != targetMusicAreaName)
        {
            currentlyTransitioningIntoMusicAreaName = targetMusicAreaName;

            switchableAudioSources.GetAudioSourceNotPlayingClip(musicAreaConfigs[currentMusicName].audioClip).volume = 0f;
            switchableAudioSources.GetAudioSourceNotPlayingClip(musicAreaConfigs[currentMusicName].audioClip).clip = musicAreaConfigs[currentlyTransitioningIntoMusicAreaName].audioClip;
            switchableAudioSources.GetAudioSourceNotPlayingClip(musicAreaConfigs[currentMusicName].audioClip).Play();
            switchableAudioSources.GetAudioSourceNotPlayingClip(musicAreaConfigs[currentMusicName].audioClip).time = musicAreaConfigs[currentlyTransitioningIntoMusicAreaName].amountPlayed;

            transitionProgress = 0f;
            isTransitioning = true;
        }

        if (isTransitioning)
        {
            // Crossfade.
            switchableAudioSources.GetAudioSourcePlayingClip(musicAreaConfigs[currentMusicName].audioClip).volume = musicAreaConfigs[currentMusicName].volumeMultiplier - (transitionProgress * musicAreaConfigs[currentMusicName].volumeMultiplier);
            switchableAudioSources.GetAudioSourceNotPlayingClip(musicAreaConfigs[currentMusicName].audioClip).volume = transitionProgress * musicAreaConfigs[currentlyTransitioningIntoMusicAreaName].volumeMultiplier;

            transitionProgress += Time.deltaTime * crossfadeTimeMultiplier;
            transitionProgress = Mathf.Clamp01(transitionProgress);

            if (transitionProgress >= 1f)
            {
                // Stop transitioning, save progress and stop playing other clip.
                isTransitioning = false;
                currentMusicName = currentlyTransitioningIntoMusicAreaName;
                musicAreaConfigs[currentMusicName].amountPlayed = switchableAudioSources.GetAudioSourceNotPlayingClip(musicAreaConfigs[currentMusicName].audioClip).time;
                switchableAudioSources.GetAudioSourceNotPlayingClip(musicAreaConfigs[currentMusicName].audioClip).Stop();
                switchableAudioSources.GetAudioSourceNotPlayingClip(musicAreaConfigs[currentMusicName].audioClip).clip = null;

                switchableAudioSources.GetAudioSourcePlayingClip(musicAreaConfigs[currentMusicName].audioClip).volume = 1f * musicAreaConfigs[currentMusicName].volumeMultiplier; // Set the volume of the now master audio source to 1f.
            }
        }
    }

    /// <summary>
    /// Notify that the player has entered a new area and to transition the music.
    /// </summary>
    /// <param name="name"></param>
    public void OnAreaEnter(string name)
    {
        Assert.IsTrue(musicAreaConfigs.ContainsKey(name), $"Invalid param {nameof(name)}. Doesn't exist in ${nameof(musicAreaUserConfig)}. Make sure that you're using the right name and that its configured in this GameObject.");
        targetMusicAreaName = name;

        // Don't setup other audio source here because a transition could be in progress.
    }
}
