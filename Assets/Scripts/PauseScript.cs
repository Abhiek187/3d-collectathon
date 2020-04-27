using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PauseScript : MonoBehaviour
{
    private TextMeshProUGUI pauseText;
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            CheckState();
        }
    }

    public void CheckState()
    {
        // Need this function when clicking on the pause button
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        pauseText.SetText("<sprite=1>");
        GameObject currentArea = GameObject.Find("FPSController").GetComponent<FirstPersonController>().m_LastArea;
        currentArea.GetComponent<AudioSource>().Pause(); // stop the audio playing in the background
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseText.SetText("<sprite=0>");
        GameObject currentArea = GameObject.Find("FPSController").GetComponent<FirstPersonController>().m_LastArea;
        currentArea.GetComponent<AudioSource>().Play(); // resume the audio playing in the background
    }
}
