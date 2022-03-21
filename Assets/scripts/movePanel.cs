using UnityEngine;
using System.Collections;


public class movePanel : Triggerable
{
    [SerializeField]
    // Transforms to act as start and end markers for the journey.
    public Transform startPoint, endPoint;

    [SerializeField]
    // Movement speed in units per second.
    public float speed = 2F;
    
    void Start()
    {
        gameObject.transform.position = startPoint.position;
    }

    private async void Move(Transform awayFrom, Transform towards) {   
        transform.position = Vector3.MoveTowards(transform.position, towards.position, speed*Time.smoothDeltaTime);    
    }

    void Update()
    {

        if (isTriggered) {
            Move(startPoint, endPoint);
        }
        else {
            Move(endPoint, startPoint);
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