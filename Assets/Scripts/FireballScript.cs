using UnityEngine;

public class FireballScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float angle;
    [SerializeField] private float radius;

    private float circleCounter = 0f;
    private Vector3 axis;
    private Vector3 center;

    // Start is called before the first frame update
    void Start()
    {
        float radians = angle * Mathf.PI / 180; // convert degrees to radians for Mathf functions
        axis = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));
        Vector3 centerAxis = new Vector3(-Mathf.Sin(radians), 0, Mathf.Cos(radians)); // -90 degrees of angle axis
        // Moving forward ex: centerAxis = Vector3.forward (0, 0, 1), axis = Vector3.right (1, 0, 0)
        center = transform.position + centerAxis * radius;
    }

    // Update is called once per frame
    void Update()
    {
        circleCounter += Time.deltaTime * speed;
        // If you looked at the fireball in the positive angle axis, it would be moving counterclockwise
        transform.position = Quaternion.AngleAxis(circleCounter, axis) * new Vector3(0, radius) + center;
    }
}
