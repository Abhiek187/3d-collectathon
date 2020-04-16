﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI progressText;

    private void Start()
    {
        if (progressText != null)
        {
            progressText.gameObject.SetActive(false); // hide progress until we load
        }
    }

    // Only public void functions appear in the event trigger
    public void SceneLoader(string scene)
    {
        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
        }
        
        StartCoroutine(AsyncSceneLoader(scene));
    }

    private IEnumerator AsyncSceneLoader(string scene)
    {
        // Show loading progress and transition to the given scene
        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);

        while (!ao.isDone)
        {
            if (progressText != null)
            {
                // ao.progress goes from 0-0.9, so clamp it from 0-1
                float progress = Mathf.Clamp01(ao.progress / 0.9f);
                progressText.SetText($"Loading: {progress * 100}%");
            }
            
            yield return null;
        }
    }
}
