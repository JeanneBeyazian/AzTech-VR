// A longer example of Vector3.Lerp usage.
// Drop this script under an object in your scene, and specify 2 other objects in the "startMarker"/"endMarker" variables in the script inspector window.
// At play time, the script will move the object along a path between the position of those two markers.

using UnityEngine;
using System.Collections;

public class movePanel : Triggerable
{
    // Transforms to act as start and end markers for the journey.
    public Transform startPoint;
    public Transform endPoint;


    // Variables start and destination to move back and forth
    private Transform movingFrom;
    private Transform movingTo;

    private float progress;


    // Movement speed in units per second.
    public float percentPerFrame = 2F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    void Start()
    {
        
        gameObject.transform.position = startPoint.position;
    
        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(startPoint.position, endPoint.position);

        movingFrom = startPoint;
        movingTo = endPoint;
    }

    // Move to the target end position.
    private void Move() {

        float distCovered = Vector3.Distance(gameObject.transform.position, startPoint.position);
        progress = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, progress+(percentPerFrame/100));
    }

    private void MoveBack() {

        float distCovered = Vector3.Distance(gameObject.transform.position, endPoint.position);
        progress = distCovered / journeyLength;
        transform.position = Vector3.Lerp(endPoint.position, startPoint.position, progress+(percentPerFrame/100));

    }

    void Update()
    {

        if (isTriggered) {
            Move();
        }
        else {
            MoveBack();
        }
        

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            other.transform.parent = transform;

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.parent = null;
        }
    }

}