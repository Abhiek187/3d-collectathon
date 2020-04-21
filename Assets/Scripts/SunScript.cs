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
    // Keys used for PlayerPrefs
    private const string gemsKey = "gems";
    private const string timeKey = "time";

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

        // Check if save data exists, else create it
        if (!PlayerPrefs.HasKey(gemsKey) || !PlayerPrefs.HasKey(timeKey))
        {
            if (!PlayerPrefs.HasKey(gemsKey)) PlayerPrefs.SetInt(gemsKey, 0); // default: 0 mini gems
            if (!PlayerPrefs.HasKey(timeKey)) PlayerPrefs.SetFloat(timeKey, float.MaxValue); // default: 10.8 nonillion years!!!
            PlayerPrefs.Save();
        }
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

            congratsText.SetText(CreateTextString(miniGemsCollected, gameTimer));
            congratsText.gameObject.SetActive(true); // show congrats message
            restartButton.SetActive(true);
            StartCoroutine(FadeInText(congratsText));

            fireworks.Play(); // show fireworks
            fireworks.GetComponent<AudioSource>().Play();
        }
    }

    private string CreateTextString(int gems, float time)
    {
        // Compare gem and time values from PlayerPrefs
        string res = $"Well Done!\n";

        if (gems > PlayerPrefs.GetInt(gemsKey))
        {
            PlayerPrefs.SetInt(gemsKey, gems);
            res += $"Gems: <sprite=5> {gems} <color=yellow>New Record!</color>\n";
        }
        else
        {
            res += $"Gems: <sprite=5> {gems} (Best: <sprite=5> {PlayerPrefs.GetInt(gemsKey)})\n";
        }

        // Get time played in minutes and seconds
        string timeStr = TimeSpan.FromSeconds(gameTimer).ToString(@"mm\:ss");

        if (time < PlayerPrefs.GetFloat(timeKey))
        {
            PlayerPrefs.SetFloat(timeKey, time);
            res += $"Time: {timeStr} <color=yellow>New Record!</color>";
        }
        else
        {
            string bestTimeStr = TimeSpan.FromSeconds(PlayerPrefs.GetFloat(timeKey)).ToString(@"mm\:ss");
            res += $"Time: {timeStr} (Best: {bestTimeStr})";
        }

        PlayerPrefs.Save();
        return res;
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
