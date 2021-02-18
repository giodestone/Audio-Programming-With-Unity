using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Plays a random sound when a click is deteccted.
/// </summary>
public class PlaySoundOnClick3D : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourceToPlayFrom;
    [SerializeField] private List<AudioClip> audioClipsToPlay;

    void Start()
    {
        Assert.IsNotNull(audioSourceToPlayFrom, $"{nameof(audioSourceToPlayFrom)} is null. Set it.");
        Assert.IsTrue(audioClipsToPlay.Count > 0, $"{nameof(audioClipsToPlay)} is empty. Play some audio clips.");
    }

    void On3DClicked()
    {
        // Play random sound on play.
        audioSourceToPlayFrom.clip = audioClipsToPlay[Random.Range(0, audioClipsToPlay.Count)];
        audioSourceToPlayFrom.Play();
    }
}
