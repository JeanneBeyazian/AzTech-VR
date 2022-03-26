using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using Ubiq.Samples;
using System.Linq;
using Ubiq.Messaging;
using UnityEngine;


public class PressurePlate : MonoBehaviour, INetworkObject, INetworkComponent, ISpawnable
{  

    public NetworkId Id { get; set; } = new NetworkId("a534-5925-5620-a196");

    public Triggerable[] triggerables;

    private NetworkContext context;
    public NetworkScene scene;

    private bool triggered;

    void Start()
    {
        context = scene.RegisterComponent(this);        
    }

    private void Awake()
    {
        triggered = false;
    }


    void OnTriggerEnter(Collider other) {

        for (int i=0; i<triggerables.Length; ++i) {
                triggerables[i].beTriggered(this);
            }
    }
    
    void OnTriggerStay(Collider other) {

        if (other.tag == "Player") {
            Debug.Log("Use Pressure Plate");
            triggered = true;
            context.SendJson(new Message(triggered));
        }

    }
    void OnTriggerExit(Collider other){
        
        for (int i=0; i<triggerables.Length; ++i) {
                triggerables[i].beTriggered(this);
            }

        triggered = false;
    }

 
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        this.triggered = msg.triggered;

    }
    public struct Message
    {
        public bool triggered;
        public Message( bool triggered)
        {
            this.triggered = triggered;
        }
    }

    public void OnSpawned(bool local){}

}