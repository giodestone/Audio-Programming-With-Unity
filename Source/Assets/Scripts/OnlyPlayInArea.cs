using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Mutes and unmutes audio sources if outside of area. Requires to be placed in a trigger collider.
/// </summary>
public class OnlyPlayInArea : MonoBehaviour
{
    [Header("Make sure that the audio source's volume is set to zero at the maximum distance otherwise the audio will pop.")]
    [SerializeField] private List<AudioSource> audioSourcesToMute;
    const string playerTag = "Player";

    void Start()
    {
        // Find player
        var player = GameObject.FindGameObjectWithTag(playerTag);
        Assert.IsNotNull(player, $"Player (tagged as {playerTag}) not found! Add a player.");

        Assert.IsNotNull(audioSourcesToMute, $"{nameof(audioSourcesToMute)} has not been set. Set it!");

        foreach (var audioSourceToMute in audioSourcesToMute)
        {
            string volumeNeverReachesZeroErrorMessage = $"The audio source which is a part of { audioSourceToMute.name } doesn't reach zero volume at distance. This will cause audio to mute as a result of this script. Set the volume to be zero at maximum distance as a result.";
            switch (audioSourceToMute.rolloffMode)
            {
                case AudioRolloffMode.Custom:
                    Assert.IsFalse(audioSourceToMute.GetCustomCurve(AudioSourceCurveType.CustomRolloff).Evaluate(audioSourceToMute.maxDistance) > 0.0001f, volumeNeverReachesZeroErrorMessage);
                    break;
                default:
                    // I can't add any extra checks because I cannot access the minimum volume value reached on the 3D Sound Settings graph.
                    break;
            }
            audioSourceToMute.mute = true;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            audioSourcesToMute.ForEach(astm => astm.mute = false);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
           audioSourcesToMute.ForEach(astm => astm.mute = true);
    }
}
