using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Threading;

public enum EnvelopeKeyFrameNames
{
    AttackLeft,
    AttackRight,
    DecayRight,
    SustainRight,
    ReleaseRight
}

[System.Serializable]
public class Envelope
{
    [SerializeField] private AnimationCurve curve;
    
    // Properties.
    public AnimationCurve Curve { get => curve; }
    public Keyframe GetKeyframe(EnvelopeKeyFrameNames name) => keyFrameMap[name];
    public List<PlayHead> PlayHeads => playHeads;


    private Dictionary<EnvelopeKeyFrameNames, Keyframe> keyFrameMap;
    private List<PlayHead> playHeads;

    private PlayHead currentPlayHead = null;

    public void Setup()
    {
        SetupKeyframeMap();
        playHeads = new List<PlayHead>();
    }

    void SetupKeyframeMap()
    {
        // Would have to ask the user to tell the program which time keyframes are which sounds tedious both for the user and this.
        Assert.IsTrue(Curve.keys.Length == 5, "There need to be exactly 5 keyframes. Make sure that there are that many!");

        keyFrameMap = new Dictionary<EnvelopeKeyFrameNames, Keyframe>();

        // Unity docs say that AnimationCurve.keys are already sorted.
        keyFrameMap.Add(EnvelopeKeyFrameNames.AttackLeft, Curve[0]);
        keyFrameMap.Add(EnvelopeKeyFrameNames.AttackRight, Curve[1]);
        keyFrameMap.Add(EnvelopeKeyFrameNames.DecayRight, Curve[2]);
        keyFrameMap.Add(EnvelopeKeyFrameNames.SustainRight, Curve[3]);
        keyFrameMap.Add(EnvelopeKeyFrameNames.ReleaseRight, Curve[4]);
    }

    public void Update(float dt)
    {   
        // Playheads are updated when the position is taken.
        playHeads.RemoveAll(playhead => playhead.IsDone);
    }

    public void KeyPressed()
    {
        if (currentPlayHead != null)
            return;

        currentPlayHead = new PlayHead(this);
        currentPlayHead.Press();
        playHeads.Add(currentPlayHead);
    }

    public void KeyReleased()
    {
        if (currentPlayHead == null)
            return;

        currentPlayHead.Release();
        currentPlayHead = null;
    }
}

public class PlayHead
{
    public float CurrentPos => currentPos;
    public bool IsDone => currentPos >= owningEnvelope.GetKeyframe(EnvelopeKeyFrameNames.ReleaseRight).time + 0.001f;

    float currentPos = 0f;
    bool isSustaining = false;
    Envelope owningEnvelope;

    public PlayHead(Envelope owningEnvelope)
    {
        this.owningEnvelope = owningEnvelope;
    }

    public void Update(float dt)
    {
        currentPos += dt;

        // If pressed continue sustaining
        if (isSustaining && currentPos >= owningEnvelope.GetKeyframe(EnvelopeKeyFrameNames.SustainRight).time)
        {
            currentPos = owningEnvelope.GetKeyframe(EnvelopeKeyFrameNames.DecayRight).time;
        }
        // If not sustaining and not past release time go to release start.
        else if (!isSustaining && currentPos >= owningEnvelope.GetKeyframe(EnvelopeKeyFrameNames.DecayRight).time && currentPos <= owningEnvelope.GetKeyframe(EnvelopeKeyFrameNames.SustainRight).time)
        {
            currentPos = owningEnvelope.GetKeyframe(EnvelopeKeyFrameNames.SustainRight).time;
        }
    }

    public void Press()
    {
        isSustaining = true;
    }

    public void Release()
    {
        isSustaining = false;
    }
}

[RequireComponent(typeof(AudioSource))]
public class SynthNotePlayer : MonoBehaviour
{
    enum EnvelopeState
    {
        Off,
        Attacking,
        Decaying,
        Sustaining,
        Releasing
    }

    enum EnvelopeKeyFrameNames
    {
        AttackLeft,
        AttackRight,
        DecayRight,
        SustainRight,
        ReleaseRight
    }

    [SerializeField] private Envelope volumeEnvelope;
    [SerializeField] private Envelope pitchEnvelope;

    [SerializeField] private float frequency = 440f;
    [SerializeField] private float gain = 0.05f;
    [SerializeField] private float maxSecondsNotPressedToConsiderReleased = 0.008f;

    private float phase = 0f; // Where on the wave are we.
    private float sampleRate;

    bool isRunning = false;

    float lastTimePressed;
    float timeToConsiderAsReleased;

    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
        volumeEnvelope.Setup();
        // pitchEnvelope.Setup();
        isRunning = true;
    }

    void Update()
    {
        var debug = Time.time;
        if (Time.time >= timeToConsiderAsReleased)
        {
            volumeEnvelope.KeyReleased();
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isRunning)
            return;
        
        if (volumeEnvelope.PlayHeads.Count < 1)
            return;

        foreach (var volumePlayHead in volumeEnvelope.PlayHeads)
        {
            var increment = frequency * 2f * Mathf.PI / sampleRate;

            for (var i = 0; i < data.Length; i = i + channels)
            {
                // Move along where we are on the sine wave.
                phase = phase + increment;
                var msPerSample = sampleRate / data.Length; // Each sample represents some amount of MS infront of the current sample

                data[i] += (gain * volumeEnvelope.Curve.Evaluate(volumePlayHead.CurrentPos)) * Mathf.Sin(phase);
                
                volumePlayHead.Update((msPerSample / 1000f) / (float)data.Length);
                // Adds data to other channels.
                for (var c = (channels - 1); c < channels; c++ /* Does this count as an implementation in c++ 😎 */)
                {
                    data[i + c] = data[i];
                }
            }
        }

        volumeEnvelope.Update(sampleRate / data.Length * 1000f);
    }

    void On3DClicked()
    {
        volumeEnvelope.KeyPressed();
        lastTimePressed = Time.time;
        timeToConsiderAsReleased = Time.time + maxSecondsNotPressedToConsiderReleased;
    }

    /*
     * Code for sine wave generation adapted from 
     * https://www.youtube.com/watch?v=GqHFGMy_51c and 
     * https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnAudioFilterRead.html.
     */
}
