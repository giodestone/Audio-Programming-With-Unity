using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioEffectZoneManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixerToAffect;
    [SerializeField] private string noEffectSnapshotName = "No Effects";
    [SerializeField] private string effectSnapshotName;
    [SerializeField] private float transitionTime = 0.25f; // Too low value will cause pops. Fixed in Unity 2020.2.

    private AudioMixerSnapshot noEffectSnapshot;
    private AudioMixerSnapshot effectSnapshot;

    private bool isTransitioning = false;
    private float transitionEndTime = 0f;
    private AudioMixerSnapshot currentSnapshotEffect;
    private AudioMixerSnapshot transitioningIntoSnapshotEffect;
    private AudioMixerSnapshot targetSnapshotEffect;

    void Start()
    {
        effectSnapshot = audioMixerToAffect.FindSnapshot(effectSnapshotName);
        noEffectSnapshot = audioMixerToAffect.FindSnapshot(noEffectSnapshotName);

        currentSnapshotEffect = noEffectSnapshot;
        targetSnapshotEffect = currentSnapshotEffect;
        currentSnapshotEffect.TransitionTo(transitionTime);
    }

    void Update()
    {
        // Check if something to transition to.
        if (!isTransitioning && currentSnapshotEffect != targetSnapshotEffect)
        {
            isTransitioning = true;
            transitioningIntoSnapshotEffect = targetSnapshotEffect;
            
            transitioningIntoSnapshotEffect.TransitionTo(transitionTime);
            
            transitionEndTime = Time.time + transitionTime;
        }
        
        // Check if finished transition to allow new transitions.
        if (isTransitioning && Time.time > transitionEndTime)
        {
            isTransitioning = false;
            currentSnapshotEffect = targetSnapshotEffect;
        }
    }

    public void BeginTransitionIntoEffectSnapshot()
    {
        targetSnapshotEffect = effectSnapshot;
    }

    public void BeginTransitionOutOfEffectSnapshot()
    {
        targetSnapshotEffect = noEffectSnapshot;
    }
}
