using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    [SerializeField] private Material nightSky;
    [SerializeField] private ParticleSystem fireworks;

    private Transform megaGems;
    private Transform miniGems;
    private int miniGemsTotal;
    private TextMeshProUGUI congratsText;
    private GameObject restartButton;
    private const float duration = 3f;
    private float intensity;
    private float gameTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        megaGems = GameObject.Find("Mega Gems").transform;
        miniGems = GameObject.Find("Mini Gems").transform;
        miniGemsTotal = miniGems.childCount;
        congratsText = GameObject.Find("Congrats").GetComponent<TextMeshProUGUI>();
        congratsText.gameObject.SetActive(false); // hide congrats message initially
        restartButton = GameObject.Find("Restart Button");
        restartButton.SetActive(false);
        intensity = GetComponent<Light>().intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (intensity != 0)
        {
            gameTimer += Time.deltaTime; // prevent the timer from going up forever
        }

        // When all the mega gems have been collected, cue to ending
        if (megaGems.childCount == 0)
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
            int miniGemsCollected = miniGemsTotal - miniGems.childCount;
            // Get time played in minutes and seconds
            string timeStr = TimeSpan.FromSeconds(gameTimer).ToString(@"mm\:ss");

            congratsText.SetText($"Well Done!\n" +
                $"High Score: <sprite=5> {miniGemsCollected}\n" +
                $"<color=red>New Record!</color>\n" +
                $"Time: {timeStr}\n" +
                $"<color=red>New Record!</color>");
            congratsText.gameObject.SetActive(true); // show congrats message
            restartButton.SetActive(true);
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
