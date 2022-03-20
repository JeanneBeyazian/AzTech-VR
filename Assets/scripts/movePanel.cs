using UnityEngine;
using System.Collections;

public class movePanel : Triggerable
{
    // Transforms to act as start and end markers for the journey.
    public Transform startPoint;
    public Transform endPoint;

    // Percent of the journey covered at each frame
    public float percentPerFrame = 2F;

    // Total distance between the markers.
    private float journeyLength;

    void Start()
    {
        gameObject.transform.position = startPoint.position;
        percentPerFrame /= 100;
        // Calculate the journey length.
        journeyLength = Vector3.Distance(startPoint.position, endPoint.position);
    }

    /*
    *   Move the platform using linear interpolation
    */
    private void Move(Transform awayFrom, Transform towards) 
    {

        float distCovered = Vector3.Distance(gameObject.transform.position, awayFrom.position);
        float progress = distCovered / journeyLength;
        transform.position = Vector3.Lerp(awayFrom.position, towards.position, progress+percentPerFrame);
    }

    void Update()
    {

        if (isTriggered) Move(startPoint, endPoint);
        else Move(endPoint, startPoint);

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            other.transform.parent = transform;
        
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            other.transform.parent = null;
        
    }

}