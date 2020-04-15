using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    [SerializeField] private Material nightSky;
    [SerializeField] private ParticleSystem fireworks;

    private Transform gems;
    private TextMeshProUGUI congratsText;
    private float duration = 5f;
    private float intensity;

    // Start is called before the first frame update
    void Start()
    {
        gems = GameObject.Find("Mega Gems").transform;
        congratsText = GameObject.Find("Congrats").GetComponent<TextMeshProUGUI>();
        congratsText.gameObject.SetActive(false); // hide congrats message initially
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
            RenderSettings.skybox = nightSky; // make the sky night
            congratsText.gameObject.SetActive(true); // show congrats message
            StartCoroutine(FadeInText(congratsText));
            fireworks.Play(); // show fireworks
            fireworks.GetComponent<AudioSource>().Play();
        }
    }

    private IEnumerator FadeInText(TextMeshProUGUI textUI)
    {
        float timer = 0f;
        Color32 startColor = new Color32(255, 255, 255, 0); // white, but alpha = 0
        Color32 endColor = new Color32(255, 255, 255, 255);

        while (timer < 1)
        {
            // Slowly increase the alpha of text
            textUI.color = Color32.Lerp(startColor, endColor, timer);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // now remove the sun (else the coroutine only runs once)
    }
}
