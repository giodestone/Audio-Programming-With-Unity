using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Configuration for setting up footsteps.
/// </summary>
[System.Serializable]
public class FootstepConfig
{
    [SerializeField] public string nameOfFootstepZone = DefaultFootstepZoneName;
    [SerializeField] public List<AudioClip> footstepSounds;
    [SerializeField] public float minPitch = 0.9f;
    [SerializeField] public float maxPitch = 1.1f;
    [SerializeField] public float minVolumeMultiplier = 0.8f;
    [SerializeField] public float maxVolumeMultiplier = 1.1f;

    public const string DefaultFootstepZoneName = "Default";
}

public class FootstepSystem : MonoBehaviour
{
    [SerializeField] private Transform rayCastFootstepOrigin; // Should be slightly above the bottom.
    [SerializeField] private float footstepFrequencyHz = 0.4f;
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private List<FootstepConfig> footstepUserConfig = new List<FootstepConfig>() { new FootstepConfig() };

    private const string playerTag = "Player";
    private FirstPersonMovement firstPersonMovement; // Reference to player controller.

    private string currentZone = FootstepConfig.DefaultFootstepZoneName;
    private float nextTimeToPlayFootstep;

    private Dictionary<string, List<FootstepConfig>> footstepZoneSounds; // Used to speed up picking random sound.
    private Dictionary<GameObject, string> gameObjectToFootstepArea; // Used to speed up collision detection.

    void Start()
    {
        GetAllFootstepZones();
        ConvertFootstepConfigToZones();
        firstPersonMovement = GameObject.FindObjectOfType<FirstPersonMovement>();
        Assert.IsNotNull(firstPersonMovement);
        Assert.IsNotNull(footstepAudioSource);
    }

    /// <summary>
    /// This gets all set footstep zones.
    /// </summary>
    void GetAllFootstepZones()
    {
        Assert.IsTrue(footstepUserConfig.Count > 0);
        gameObjectToFootstepArea = new Dictionary<GameObject, string>();
        var footstepAreas = GameObject.FindObjectsOfType<FootstepArea>();
        foreach (var footstepArea in footstepAreas)
        {
            gameObjectToFootstepArea.Add(footstepArea.gameObject, footstepArea.FootstepZone);
        }
    }

    /// <summary>
    /// Converts <see cref="footstepZoneSounds"/> into a dictionary for easier and quicker access for when picking random footstep sound.
    /// Unity does not contain a serializable dictionary.
    /// </summary>
    void ConvertFootstepConfigToZones()
    {
        footstepZoneSounds = new Dictionary<string, List<FootstepConfig>>();
        foreach (var config in footstepUserConfig)
        {
            if (footstepZoneSounds.ContainsKey(config.nameOfFootstepZone))
            {
                footstepZoneSounds[config.nameOfFootstepZone].Add(config);
            }
            else
            {
                footstepZoneSounds.Add(config.nameOfFootstepZone, new List<FootstepConfig>(new[] { config }));
            }
        }
    }

    void Update()
    {
        UpdateCurrentZoneFromSurface();
        PlayFootstepSound();
    }
    
    /// <summary>
    /// Raycast below player and update <see cref="currentZone"/> based on what was hit.
    /// </summary>
    void UpdateCurrentZoneFromSurface()
    {
        Physics.Raycast(rayCastFootstepOrigin.position, -rayCastFootstepOrigin.up, out var hit, 4f); // Ignores Self.-
        if (gameObjectToFootstepArea.ContainsKey(hit.collider.gameObject))
        {
            currentZone = gameObjectToFootstepArea[hit.collider.gameObject];
        }
        
    }

    void PlayFootstepSound()
    {
        if (firstPersonMovement.IsMoving)
        {
            var debug = Time.time;
            if (Time.time >= nextTimeToPlayFootstep)
            {
                var sounds = footstepZoneSounds[currentZone];
                var pickedSound = sounds[Random.Range(0, sounds.Count)];
                
                footstepAudioSource.pitch = Random.Range(pickedSound.minPitch, pickedSound.maxPitch);
                footstepAudioSource.volume = Random.Range(pickedSound.minVolumeMultiplier, pickedSound.maxVolumeMultiplier);

                var pickedClip = pickedSound.footstepSounds[Random.Range(0, pickedSound.footstepSounds.Count)];

                footstepAudioSource.PlayOneShot(pickedClip);
                
                nextTimeToPlayFootstep = Time.time + footstepFrequencyHz;
            }
        }
        else
        {
            footstepAudioSource.Stop();
        }
    }
}
