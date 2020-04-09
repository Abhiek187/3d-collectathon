using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    [SerializeField] private Material nightSky;
    private float duration = 5f;
    private float intensity;

    // Start is called before the first frame update
    void Start()
    {
        intensity = GetComponent<Light>().intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (intensity > 0)
        {
            intensity -= Time.deltaTime / duration;
        }
        else if (intensity < 0)
        {
            Destroy(gameObject);
            RenderSettings.skybox = nightSky;
        }
    }
}
