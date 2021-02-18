using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calls functions in <see cref="AudioEffectZoneManager"/> when the player enters and exits the trigger collider.
/// </summary>
public class AudioEffectZone : MonoBehaviour
{
    [SerializeField] private AudioEffectZoneManager audioEffectZoneManager;
    const string playerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            audioEffectZoneManager.BeginTransitionIntoEffectSnapshot();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            audioEffectZoneManager.BeginTransitionOutOfEffectSnapshot();
    }
}
