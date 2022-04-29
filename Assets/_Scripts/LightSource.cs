using UnityEngine;

public class LightSource : MonoBehaviour
{
    [SerializeField] private float MaxIntensity = 1;
    
    private Light source;

    void Awake()
    {
        source = GetComponent<Light>();
        source.intensity = 0f;
    }

    public void ChangeIntensity(float percent)
    {
        // Change intensity of light depending on how far away it is from the player
        float newIntesity = percent * MaxIntensity;

        // Disable the light if its far away enough to improve performance
        if (newIntesity <= 0.05f)
        {
            source.enabled = false;
        }
        else
        {
            source.enabled = true;
            source.intensity = newIntesity;
        }
    }
}