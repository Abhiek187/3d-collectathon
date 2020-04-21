using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatformScript : MonoBehaviour
{
    [SerializeField] private float speed;

    private float timer = 0f;
    private Transform start;
    private Transform end;
    private enum State { ToStart, ToEnd, AtStart, AtEnd }; // the 4 parts of the animation
    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        start = transform.parent.GetChild(1);
        end = transform.parent.GetChild(2);
        currentState = State.ToEnd; // start going to the end point
    }

    // FixedUpdate needed to move the player with the platform
    void FixedUpdate()
    {
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
                transform.position = Vector3.MoveTowards(transform.position, end.position, Time.fixedDeltaTime * speed);
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
                transform.position = Vector3.MoveTowards(transform.position, start.position, Time.fixedDeltaTime * speed);
            }
            else
            {
                currentState = State.AtStart; // we reached the start, now rest
            }
        }
        else
        {
            timer += Time.fixedDeltaTime;

            if (timer >= 1)
            {
                // Reset the timer and change state
                timer = 0;
                currentState = currentState == State.AtStart ? State.ToEnd : State.ToStart;
            }
        }
    }
}
