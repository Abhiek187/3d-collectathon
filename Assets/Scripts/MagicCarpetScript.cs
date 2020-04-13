using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// This is similar to FloatingPlatformScript, but can rotate in multiple directions
public class MagicCarpetScript : MonoBehaviour
{
    [SerializeField] private float speed;

    private float timer = 0f;
    private Transform nextPoint;
    private int nextIndex = 2;
    private Vector3 nextPosition;
    private enum State { Moving, AtRest }; // the 2 parts of the animation
    private State currentState;
    private bool onCarpet = false;

    // Start is called before the first frame update
    void Start()
    {
        nextPoint = transform.parent.GetChild(nextIndex);
        nextPosition = nextPoint.position + Vector3.up; // stay above the waypoint markers
        currentState = State.Moving; // start going to each waypoint
    }

    // Update is called once per frame
    void Update()
    {
        onCarpet = GameObject.Find("FPSController").GetComponent<FirstPersonController>().onCarpet;
        if (!onCarpet) return; // only move the carpet if the player is on it

        /* Carpet animation:
         * 1. Start at start point
         * 2. Turn and move to each waypoint
         * 3. At the end, stop for a second
         * 4. Turn around and head back to start
         */
        if (currentState == State.Moving)
        {
            if (Vector3.Distance(transform.position, nextPosition) > 0)
            {
                // Rotate in the direction of next point
                Vector3 direction = (nextPosition - transform.position).normalized;
                Quaternion angle = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, angle,
                    Time.deltaTime * speed);

                transform.position = Vector3.MoveTowards(transform.position, nextPosition,
                    Time.deltaTime * speed);
            }
            else
            {
                nextIndex++;

                // Turn to the next waypoint, or stop if at the end
                if (nextIndex == transform.parent.childCount)
                {
                    currentState = State.AtRest;
                }
                else
                {
                    nextPoint = transform.parent.GetChild(nextIndex);
                    nextPosition = nextPoint.position +
                        (nextPoint.name == "End Point" ? Vector3.zero : Vector3.up);
                }
            }
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= 1)
            {
                // Reset the timer and head back to start
                timer = 0;
                currentState = State.Moving;
                nextIndex = 1;
                nextPoint = transform.parent.GetChild(nextIndex);
                nextPosition = nextPoint.position;
            }
        }
    }
}
