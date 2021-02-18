using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public enum CurrentTime
{
    Day,
    Night
}

public class DayNightLightingController : MonoBehaviour
{
    [SerializeField] private Light daytimeLight;
    [SerializeField] private Light nightTimeLight;
    [SerializeField] private Image blackoutImage;
    [SerializeField] private float transitionMultiplier = 1f; // How quick to transition between day and night.
    [SerializeField] List<GameObject> gameObjectsToNotifyOfTimeChange;

    private Color transparent = new Color(0f, 0f, 0f, 0f);
    private Color opaque = new Color(0f, 0f, 0f, 1f);
    private CurrentTime currentTime = CurrentTime.Day;
    private bool isTransitioning = false;
    private float transitionProgress;
    private bool hasSwitchedTimeOfDay;

    void Start()
    {
        Assert.IsNotNull(daytimeLight);
        Assert.IsNotNull(nightTimeLight);
    }

    void On3DClicked()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        hasSwitchedTimeOfDay = false;
        transitionProgress = 0f;
    }

    void Update()
    {
        if (!isTransitioning) return;

        transitionProgress += transitionMultiplier * Time.deltaTime;

        if (transitionProgress >= 0.5f && !hasSwitchedTimeOfDay)
        {
            ToggleDayNight();
            hasSwitchedTimeOfDay = true;
        }

        // Update blackout image.
        if (transitionProgress <= 0.5f)
        {
            blackoutImage.color = Color.Lerp(transparent, opaque, transitionProgress * 2f);
        }
        else
        {
            blackoutImage.color = Color.Lerp(opaque, transparent, (transitionProgress - 0.5f) * 2f);
        }

        if (transitionProgress >= 1f)
        {
            isTransitioning = false;
            blackoutImage.color = transparent;
        }
    }

    void ToggleDayNight()
    {
        switch (currentTime)
        {
            case CurrentTime.Day:
                currentTime = CurrentTime.Night;
                daytimeLight.gameObject.SetActive(false);
                nightTimeLight.gameObject.SetActive(true);
                break;
            case CurrentTime.Night:
                currentTime = CurrentTime.Day;
                daytimeLight.gameObject.SetActive(true);
                nightTimeLight.gameObject.SetActive(false);
                break;
        }

        NotifyGameObjectsOfTimeChange();
    }

    void NotifyGameObjectsOfTimeChange()
    {
        foreach (var obj in gameObjectsToNotifyOfTimeChange)
        {
            obj.SendMessage("OnTimeChanged", currentTime, SendMessageOptions.DontRequireReceiver);
        }
    }
}
