using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerEffectNotifierOnClick3D : MonoBehaviour
{
    [SerializeField] private PlayerEffect playerEffectOnClick;

    private PlayerEffectManager playerEffectManager;

    void Start()
    {
        playerEffectManager = GameObject.FindObjectOfType<PlayerEffectManager>();

        Assert.IsNotNull(playerEffectManager, $"Cannot find {nameof(PlayerEffectManager)}!");
    }

    void On3DClicked()
    {
        playerEffectManager.TransitionTo(playerEffectOnClick);
    }
}
