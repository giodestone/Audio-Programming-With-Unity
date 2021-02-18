using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// For an object with a collider which sets the music zone when the player walks into it.
/// </summary>
public class AmbientMusicZone : MonoBehaviour
{
    [SerializeField] private string nameOfAmbientAudioZoneEntry;
    [SerializeField] private string nameOfAmbientAudioZoneExit;
    AmbientMusicZoneManager ambientMusicZoneManager;

    const string playerTag = "Player";

    void Start()
    {
        ambientMusicZoneManager = GameObject.FindObjectOfType<AmbientMusicZoneManager>();
        Assert.IsNotNull(ambientMusicZoneManager, $"Unable to find a {nameof(AmbientMusicZoneManager)} in the scene. Add one.");
        Assert.IsNotNull(GameObject.FindGameObjectWithTag(playerTag), $"Item tagged {playerTag} not found! Add a player and/or tag it as such.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (nameOfAmbientAudioZoneEntry == "")
            return;

        if (other.gameObject.CompareTag(playerTag))
        {
            ambientMusicZoneManager.OnAreaEnter(nameOfAmbientAudioZoneEntry);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (nameOfAmbientAudioZoneExit == "")
            return;

        if (other.gameObject.CompareTag(playerTag))
        {
            ambientMusicZoneManager.OnAreaEnter(nameOfAmbientAudioZoneExit);
        }
    }
}
