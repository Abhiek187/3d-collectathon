using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballScript : MonoBehaviour
{
    private Vector3 originalSize;
    private Vector3 originalPosition;
    private Rigidbody rigidBody;

    private const float growRate = 0.01f;
    private const float meltRate = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        originalSize = transform.localScale;
        originalPosition = transform.position + Vector3.up; // drop ball down on respawn
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -5 || Vector3.Distance(transform.localScale, Vector3.zero) < 0.5)
        {
            transform.position = originalPosition; // respawn if fallen or melted
            transform.localScale = originalSize; // reset snowball's size
            rigidBody.velocity = Vector3.zero; // stop as well
            rigidBody.angularVelocity = Vector3.zero;
        }

        // Snowball increases in size as it's rolled around the snow
        if (transform.position.x <= 25 && transform.position.z >= 50)
        {
            transform.localScale +=
                Vector3.one * Time.deltaTime * rigidBody.velocity.magnitude * growRate;
        }
        // Else it melts (at a quicker rate) if not in the cold
        else
        {
            transform.localScale -=
                Vector3.one * Time.deltaTime * rigidBody.velocity.magnitude * meltRate;
        }
    }
}
