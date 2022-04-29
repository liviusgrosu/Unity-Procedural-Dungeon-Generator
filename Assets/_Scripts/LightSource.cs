using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour
{
    [SerializeField] float MaxIntensity = 1;
    
    private Light source;

    private void Awake()
    {
        source = GetComponent<Light>();
        source.intensity = 0f;
    }

    public void ChangeIntensity(float percent)
    {
        source.intensity = percent * MaxIntensity;
    }
}