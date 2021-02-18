using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Changes the reticle color based on whether its in range of a clickable item. Also sends clicked event.
/// </summary>
public class Click3DTriggerScript : MonoBehaviour
{
    [SerializeField] private float clickRange = 2f;
    [SerializeField] private Image reticle;
    [SerializeField] private Color clickableInRangeReticleColor = Color.blue;
    [SerializeField] private Color nothingClickableInRangeReticleColor = Color.gray;
    new Camera camera;

    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        Assert.IsNotNull(camera, "Cannot find camera in children!");
        Assert.IsNotNull(reticle);
    }


    void Update()
    {
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out var hitInfo, clickRange))
        {
            if (hitInfo.collider.CompareTag("Interactable"))
            {
                reticle.color = clickableInRangeReticleColor;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                hitInfo.collider.gameObject.SendMessage("On3DClicked", SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            // Set reticle color back.
            reticle.color = nothingClickableInRangeReticleColor;
        }

    }
}
