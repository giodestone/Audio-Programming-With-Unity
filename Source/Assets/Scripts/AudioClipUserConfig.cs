using UnityEngine;

[System.Serializable]
public class AudioClipUserConfig
{
    [SerializeField] public string audioAreaName;
    [SerializeField] public AudioClip audioClip;
    [SerializeField] public float volumeMultiplier = 1f;

    public AudioClipUserConfig(string audioAreaName)
    {
        this.audioAreaName = audioAreaName;
    }
}