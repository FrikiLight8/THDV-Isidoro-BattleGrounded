using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Transform currentTarget; // The current target transform

    void Start()
    {
        // Initialize the current target as the platform's initial position
        currentTarget = pointA; // Start by moving towards pointA
    }

    void Update()
    {
        // Move the platform toward the current target
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Check if the platform has reached the current target
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        // Switch the target to the opposite point
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
    }

    private void OnDrawGizmos()
    {
        // Draw lines in the editor to visualize the points
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
