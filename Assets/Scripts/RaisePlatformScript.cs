using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaisePlatformScript : MonoBehaviour
{
    [SerializeField] private float raisePlatformTime;

    public int buttonsPressed = 0;
    private bool platformRaised = false;

    // Update is called once per frame
    void Update()
    {
        // Raise hidden platform if all buttons are pressed
        if (buttonsPressed == 3 && !platformRaised)
        {
            StartCoroutine(RaisePlatform());
            platformRaised = true;
        }
    }

    private IEnumerator RaisePlatform()
    {
        Vector3 endPosition = transform.position + Vector3.up * 5.1f;
        float timer = 0f;

        while (Vector3.Distance(transform.position, endPosition) > 0.01)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Slerp(transform.position, endPosition, timer / raisePlatformTime);
            yield return null;
        }
    }
}
