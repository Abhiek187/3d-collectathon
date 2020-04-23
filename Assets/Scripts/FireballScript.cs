using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
    [SerializeField] private float speed;

    private float circleCounter = 0f;
    private const float radius = 5f;
    private Vector3 center;

    // Start is called before the first frame update
    void Start()
    {
        center = transform.position + Vector3.forward * radius;
    }

    // Update is called once per frame
    void Update()
    {
        circleCounter += Time.deltaTime * speed;
        transform.position = Quaternion.AngleAxis(circleCounter, Vector3.right) * new Vector3(0, radius) + center;
    }
}
