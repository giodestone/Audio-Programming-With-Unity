using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepArea : MonoBehaviour
{
    [SerializeField] private string footstepZone = "Default";

    public string FootstepZone { get => footstepZone; }
}
