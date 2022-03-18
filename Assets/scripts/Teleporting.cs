using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;
using UnityEngine;

public class Teleporting : MonoBehaviour, INetworkObject, INetworkComponent
{   

    public GameObject player;
    public Transform teleportTarget;
    public Material entry;
    public Material exit;
    public Material inactive;
    private Hand attached;
    private NetworkContext context;
    private Rigidbody body;

    // NetworkId INetworkObject.id => new NetworkId(1001);

    public struct Message
    {
       public Vector3 position;
    }
    private void Awake()
    {
        body = GetComponent<Rigidbody>();

    }

    public NetworkId Id { get; set; } 

    private void Start()
    {
        context = NetworkScene.Register(this);
        Material[] materials = {inactive, inactive};
        this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials = materials;
        // this.gameObject.GetComponent<MeshRenderer>().materials[0] = inactive;
        // this.gameObject.GetComponent<MeshRenderer>().materials[1] = inactive;     

    }


    public void Attach(Hand hand)
    {
        attached = hand;
      
    }
    void OnTriggerEnter(Collider other)
    {   
        if (teleportTarget) {
            Vector3 targetPos = teleportTarget.transform.position;
            targetPos.x -=1f;
            targetPos.z +=1.5f;
            player.transform.position = targetPos;
        }
    }
    private void Update()
    {
        if(attached)
        {
            transform.localPosition = attached.transform.position;
            // transform.rotation = attached.transform.rotation;

            Message message;
            message.position = transform.localPosition;
            context.SendJson(message);
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
      
        transform.localPosition = msg.position; // The Message constructor will take the *local* properties of the passed transform.
        // transform.localRotation = msg.transform.rotation;
       
    }

    public void OnSpawned(bool local)
    {

    }


}
