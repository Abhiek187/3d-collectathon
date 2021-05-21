using UnityEngine;

public class MiniGemScript : MonoBehaviour
{
    private Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        // Keep rotating the gem
        parent.Rotate(0, 20 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "FPSController")
        {
            // Player touched the gem, remove from the scene
            AudioSource.PlayClipAtPoint(parent.GetComponent<AudioSource>().clip, transform.position);
            Destroy(parent.gameObject);
        }
    }
}
