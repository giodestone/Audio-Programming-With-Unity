using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenSoundTrigger : MonoBehaviour
{
    [SerializeField] private TransitionableAudioPlayer transitionableAudioPlayerToNotify;


    /// <summary>
    /// Receiver for SendMessage from <see cref="KnobScript"/>. Notifies <see cref="transitionableAudioPlayerToNotify"/> to change
    /// sounds depending on the value of <paramref name="knobPosition"/>.
    /// </summary>
    /// <param name="knobPosition"></param>
    void OnKnobPositionChanged(KnobPosition knobPosition)
    {
        switch(knobPosition)
        {
            case KnobPosition.On:
                transitionableAudioPlayerToNotify.TransitionInto();
                break;

            case KnobPosition.Off:
                transitionableAudioPlayerToNotify.TransitionOut();
                break;
        }
    }
}
