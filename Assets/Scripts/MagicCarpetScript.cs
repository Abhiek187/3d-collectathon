using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// This is similar to FloatingPlatformScript, but can rotate in multiple directions
public class MagicCarpetScript : MonoBehaviour
{
    [SerializeField] private float speed;

    private float timer = 0f;
    private Transform start;
    private Transform end;
    private enum State { ToStart, ToEnd, AtStart, AtEnd }; // the 4 parts of the animation
    private State currentState;
    private bool onCarpet = false;

    // Start is called before the first frame update
    void Start()
    {
        start = transform.parent.GetChild(1);
        end = transform.parent.GetChild(2);
        currentState = State.ToEnd; // start going to the end point
    }

    // Update is called once per frame
    void Update()
    {
        onCarpet = GameObject.Find("FPSController").GetComponent<FirstPersonController>().onCarpet;
        if (!onCarpet) return;
        /* Platform animation:
         * 1. Move to end point
         * 2. Stop for 1 second
         * 3. Move to start point
         * 4. Stop for another second
         */
        if (currentState == State.ToEnd)
        {
            if (Vector3.Distance(transform.position, end.position) > 0)
            {
                // Rotate in the direction of next point
                Vector3 direction = (end.position - transform.position).normalized;
                Quaternion angle = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, angle,
                    Time.deltaTime * speed);

                transform.position = Vector3.MoveTowards(transform.position, end.position, Time.deltaTime * speed);
            }
            else
            {
                currentState = State.AtEnd; // we reached the end, now rest
            }
        }
        else if (currentState == State.ToStart)
        {
            if (Vector3.Distance(transform.position, start.position) > 0)
            {
                // Rotate in the direction of next point
                Vector3 direction = (start.position - transform.position).normalized;
                Quaternion angle = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, angle,
                    Time.deltaTime * speed);

                transform.position = Vector3.MoveTowards(transform.position, start.position, Time.deltaTime * speed);
            }
            else
            {
                currentState = State.AtStart; // we reached the start, now rest
            }
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= 1)
            {
                // Reset the timer and change state
                timer = 0;
                currentState = currentState == State.AtStart ? State.ToEnd : State.ToStart;
            }
        }
    }
}
