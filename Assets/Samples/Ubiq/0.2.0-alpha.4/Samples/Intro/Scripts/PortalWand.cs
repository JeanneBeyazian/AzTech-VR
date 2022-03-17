using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using Ubiq.Samples;
using System.Linq;
using Ubiq.Messaging;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
public class PortalWand : MonoBehaviour, IUseable, IGraspable, INetworkObject, INetworkComponent
{   
    public GameObject thePortal;
    private Rigidbody body;
    private Hand grasped;
    
    private NetworkContext context;
    // NetworkId INetworkObject.id => new NetworkId(1002);
    // Start is called before the first frame update
    public NetworkId Id { get; set; }
    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        context = NetworkScene.Register(this);
    }

    public void Grasp(Hand controller)
    {
        grasped = controller;
    }


    public void Release(Hand controller)
    {
        grasped = null;
    }

    public void UnUse(Hand controller)
    {
       
    }

    public void Use(Hand controller)
    {
        // var p = NetworkSpawner.SpawnPersistent(this, thePortal).GetComponents<MonoBehaviour>().Where(mb => mb is IPortal).FirstOrDefault() as IPortal;
        // print(0);
        // if (p != null)
        // {
        //     print(1);
        //     p.Attach(controller);
        // }
    }

    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
      
        transform.localPosition = msg.position; // The Message constructor will take the *local* properties of the passed transform.
        // transform.localRotation = msg.transform.rotation;
       
    }

    private void Update()
    {
        if (grasped != null)
        {
            transform.localPosition = grasped.transform.position;
            
            Message message;
            message.position = transform.localPosition;
            context.SendJson(message);

            body.isKinematic = true;
        }
        else
        {
            body.isKinematic = false;
        }
    }

    public struct Message
    {
       public Vector3 position;
    }

}
