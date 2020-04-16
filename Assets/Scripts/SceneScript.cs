using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour
{
    public void SceneLoader(int sceneIndex)
    {
        // Transition to the given scene
        SceneManager.LoadScene(sceneIndex);
    }
}
