using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum KnobPosition
{
    Off,
    On
}

public class KnobScript : MonoBehaviour
{
    [SerializeField] private GameObject turningCylinder;
    private Vector3 turningCylunderOffRotation = new Vector3(90f, 0f, 0f);
    private Vector3 turningCylinderOnRotation = new Vector3(0f, 90f, 90f);
    private float turningCylinderProgress = 0f;
    private float pickedClipPlayEndTime;
    private float pickedClipLength = 1f; // 1f so no divide by zero exceptions get raised.
    private AudioSource audioSource;

    [SerializeField] private MeshRenderer meshRendererMaterialToChange;
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material targetMaterial;
    [SerializeField] private float transitionSpeedMultiplier = 0.1f;
    [SerializeField] private List<AudioClip> knobSounds;
    [SerializeField] private List<GameObject> objectsToNotifyOfKnobPositionChange;
    private KnobPosition transitionState = KnobPosition.Off;
    private float progressToTargetMaterial = 0f;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(audioSource);
        Assert.IsNotNull(meshRendererMaterialToChange);
        Assert.IsNotNull(originalMaterial);
        Assert.IsNotNull(targetMaterial);
    }

    void Update()
    {
        switch (transitionState)
        {
            case KnobPosition.Off:
                progressToTargetMaterial -= transitionSpeedMultiplier * Time.deltaTime;
                turningCylinderProgress = ((pickedClipPlayEndTime - Time.time) / pickedClipLength);
                break;
            case KnobPosition.On:
                progressToTargetMaterial += transitionSpeedMultiplier * Time.deltaTime;
                turningCylinderProgress = 1f - (pickedClipPlayEndTime - Time.time) / pickedClipLength;
                break;
        }

        progressToTargetMaterial = Mathf.Clamp01(progressToTargetMaterial);
        turningCylinderProgress = Mathf.Clamp01(turningCylinderProgress);

        meshRendererMaterialToChange.material.Lerp(originalMaterial, targetMaterial, progressToTargetMaterial);
        turningCylinder.transform.rotation = Quaternion.Euler(Vector3.Lerp(turningCylunderOffRotation, turningCylinderOnRotation, turningCylinderProgress));
    }

    public void On3DClicked()
    {
        // Dont register click until the knob has finished turning.
        if (Time.time <= pickedClipPlayEndTime)
            return;

        // Invert transition state.
        switch (transitionState)
        {
            case KnobPosition.Off:
                transitionState = KnobPosition.On;
                break;
            case KnobPosition.On:
                transitionState = KnobPosition.Off;
                break;
        }

        PlayRandomSound(out pickedClipLength);
        pickedClipPlayEndTime = Time.time + pickedClipLength; // Adapt knob turn time to clip length.
        
        objectsToNotifyOfKnobPositionChange.ForEach(obj => obj.SendMessage("OnKnobPositionChanged", transitionState, SendMessageOptions.DontRequireReceiver));
    }

    public void PlayRandomSound(out float length)
    {
        var sound = knobSounds[Random.Range(0, knobSounds.Count)];
        length = sound.length;
        audioSource.PlayOneShot(sound);
    }
}
