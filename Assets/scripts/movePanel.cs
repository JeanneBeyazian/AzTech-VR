using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;
using UnityEngine;

public class movePanel : MonoBehaviour, INetworkObject, INetworkComponent
{
    //amended from https://www.youtube.com/watch?v=Ig4Gsm1QwoU


    [SerializeField]
    float speed;

    [SerializeField]
    Transform startPoint, endPoint;

    [SerializeField]
    float changeDirectionDelay;

    //added
    NetworkId INetworkObject.Id => new NetworkId(1008);
    private NetworkContext context;

    private Transform destinationTarget, departTarget;

    private float startTime;

    private float journeyLength;

    bool isWaiting;

    //added
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        transform.localPosition = msg.position;
    }

    void Start()
    {
        context = NetworkScene.Register(this);
        departTarget = startPoint;
        destinationTarget = endPoint;

        startTime = Time.time;
        journeyLength = Vector3.Distance(departTarget.position, destinationTarget.position);
    }


    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {


        if (!isWaiting)
        {
            if (Vector3.Distance(transform.position, destinationTarget.position) > 0.01f)
            {
                float distCovered = (Time.time - startTime) * speed;

                float fractionOfJourney = distCovered / journeyLength;

                transform.position = Vector3.Lerp(departTarget.position, destinationTarget.position, fractionOfJourney);
            }
            else
            {
                isWaiting = true;
                StartCoroutine(changeDelay());
            }
            //added
            Message message;
            message.position = transform.localPosition;
            context.SendJson(message);
        }


    }

    void ChangeDestination()
    {

        if (departTarget == endPoint && destinationTarget == startPoint)
        {
            departTarget = startPoint;
            destinationTarget = endPoint;
        }
        else
        {
            departTarget = endPoint;
            destinationTarget = startPoint;
        }

    }
    IEnumerator changeDelay()
    {
        yield return new WaitForSeconds(changeDirectionDelay);
        ChangeDestination();
        startTime = Time.time;
        journeyLength = Vector3.Distance(departTarget.position, destinationTarget.position);
        isWaiting = false;
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

    public struct Message
    {
        //public TransformMessage transform;
        public Vector3 position;
        //public Message(Transform transform)
        //{
            //this.transform = new TransformMessage(transform);
            

        //}
    }

}
