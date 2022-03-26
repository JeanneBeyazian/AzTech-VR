using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using Ubiq.Samples;
using System.Linq;
using Ubiq.Messaging;
using UnityEngine;


public class MovePanel : Triggerable, INetworkObject, INetworkComponent, ISpawnable
{
    [SerializeField]
    // Transforms to act as start and end markers for the journey.
    public Transform startPoint, endPoint;

    [SerializeField]
    // Movement speed in units per second.
    public float speed = 2F;
    
    private NetworkContext context;



    void Start()
    {
        context = NetworkScene.Register(this);
        gameObject.transform.position = startPoint.position;
    }

    private async void Move(Transform awayFrom, Transform towards) {   
        transform.position = Vector3.MoveTowards(transform.position, towards.position, speed*Time.smoothDeltaTime);  
        //context.SendJson(new Message(transform));
  
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

    // Network Unit
    // public NetworkId Id { get; set; } = new NetworkId("95dd645c-20789b8e");
    public NetworkId Id { get; set; }

    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
  
        transform.position = msg.transform.position; // The Message constructor will take the *local* properties of the passed transform.
       
    }
    public struct Message
    {
        public TransformMessage transform;
        public Message(Transform transform)
        {
            this.transform = new TransformMessage(transform);
        }
    }
    public void OnSpawned(bool local)
    {
    }

}