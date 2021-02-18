using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Triggers the squeal effect (<see cref="SynthPoisonedSqueakPlayer"/>) based on the message
/// received from <see cref="PlayerEffectManager"/>.
/// </summary>
public class PoisonSquealEffectTrigger : MonoBehaviour
{
    SynthPoisonedSqueakPlayer synthPoisonedSqueakPlayer;
    void Start()
    {
        synthPoisonedSqueakPlayer = GameObject.FindObjectOfType<SynthPoisonedSqueakPlayer>();
        Assert.IsNotNull(synthPoisonedSqueakPlayer);
    }

    void OnPlayerEffectTransitionBegin(PlayerEffectOnTransitionArgs args)
    {
        switch (args.TargetEffect)
        {
            case PlayerEffect.None:
                synthPoisonedSqueakPlayer.StopPlayingSound(args.TransitionTimeSeconds);
                break;

            case PlayerEffect.Poisoned:
                synthPoisonedSqueakPlayer.BeginPlayingSound(args.TransitionTimeSeconds);
                break;
        }
    }
}
