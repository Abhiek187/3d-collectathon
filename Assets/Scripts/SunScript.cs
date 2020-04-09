using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    [SerializeField] private Material nightSky;

    private Transform gems;
    private float duration = 5f;
    private float intensity;

    // Start is called before the first frame update
    void Start()
    {
        gems = GameObject.Find("Mega Gems").transform;
        intensity = GetComponent<Light>().intensity;
    }

    // Update is called once per frame
    void Update()
    {
        // When all the mega gems have been collected, cue to ending
        if (gems.childCount == 0)
        {
            NightTransition();
        }
    }

    private void NightTransition()
    {
        if (intensity > 0)
        {
            intensity -= Time.deltaTime / duration;
        }
        else if (intensity < 0)
        {
            intensity = 0;
            Destroy(gameObject);
            RenderSettings.skybox = nightSky;
        }
    }
}
