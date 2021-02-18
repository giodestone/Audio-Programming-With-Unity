using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Assertions;

/// <summary>
/// Contains all the possible effects a player can have enacted on them at one time.
/// </summary>
public enum PlayerEffect
{
    None,
    Poisoned,
    Drunk
}

/// <summary>
/// Config for the player effect.
/// </summary>
[System.Serializable]
public class PlayerEffectConfig
{
    public PlayerEffect playerEffect;
    public Color playerImageEffectColor;
    public AudioMixerSnapshot snapshotToTransitionTo;
    public List<AudioClip> transitionIntoSoundEffects;
    public float transitionTimeSeconds = 0.5f;
}

public struct PlayerEffectOnTransitionArgs
{
    public float TransitionTimeSeconds;
    public PlayerEffect TargetEffect;
}

/// <summary>
/// Manages the effects on the player.
/// </summary>
public class PlayerEffectManager : MonoBehaviour
{
    [SerializeField] private List<PlayerEffectConfig> playerEffectUserConfigs;
    [SerializeField] private AudioSource audioSourceForTransitionEffect;
    [SerializeField] private Image playerEffectImage;
    [SerializeField] private List<GameObject> gameObjectsToNotifyOfTransition;

    private const string notifyObjectsFunctionName = "OnPlayerEffectTransitionBegin";

    private Dictionary<PlayerEffect, PlayerEffectConfig> playerEffectConfigs;
    
    bool isTransitioning = false;
    float transitionEndTime;
    Color originalEffectImageColor;
    Color targetPlayerEffectImageColor;
    PlayerEffect currentEffect;

    void Start()
    {
        Assert.IsNotNull(audioSourceForTransitionEffect);
        Assert.IsNotNull(playerEffectImage);
        Assert.IsTrue(playerEffectUserConfigs.Count > 0, "No effects set, set them up!");

        ConvertPlayerConfigsToDictionary();
    }

    void ConvertPlayerConfigsToDictionary()
    {
        playerEffectConfigs = new Dictionary<PlayerEffect, PlayerEffectConfig>(playerEffectUserConfigs.Count);

        foreach (var userConfig in playerEffectUserConfigs)
        {
            playerEffectConfigs.Add(userConfig.playerEffect, userConfig);
        }
    }

    void Update()
    {
        if (isTransitioning)
        {
            playerEffectImage.color = Color.Lerp(originalEffectImageColor, targetPlayerEffectImageColor, Time.time / transitionEndTime);
            isTransitioning = Time.time <= transitionEndTime;
        }
    }

    /// <summary>
    /// Transition to another effect. If already transitioning this request will be ignored.
    /// </summary>
    /// <param name="effectToTransitionTo"></param>
    public void TransitionTo(PlayerEffect effectToTransitionTo)
    {
        if (isTransitioning) // No fancy system as it would be weird.
            return;

        if (currentEffect == effectToTransitionTo) // Don't let the player drink stuff too much.
            return;

        isTransitioning = true;

        var effectConfig = playerEffectConfigs[effectToTransitionTo];
        currentEffect = effectToTransitionTo;
        audioSourceForTransitionEffect.PlayOneShot(effectConfig.transitionIntoSoundEffects[Random.Range(0, effectConfig.transitionIntoSoundEffects.Count)]);
        transitionEndTime = Time.time + effectConfig.transitionTimeSeconds;
        
        originalEffectImageColor = playerEffectImage.color;
        targetPlayerEffectImageColor = effectConfig.playerImageEffectColor;

        effectConfig.snapshotToTransitionTo.TransitionTo(effectConfig.transitionTimeSeconds);
        
        var args = new PlayerEffectOnTransitionArgs { TargetEffect = effectToTransitionTo, TransitionTimeSeconds = effectConfig.transitionTimeSeconds };
        gameObjectsToNotifyOfTransition.ForEach(obj => obj.SendMessage(notifyObjectsFunctionName, args, SendMessageOptions.DontRequireReceiver));
    }
}
