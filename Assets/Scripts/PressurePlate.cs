using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using Ubiq.Samples;
using System.Linq;
using Ubiq.Messaging;
using UnityEngine;


public class PressurePlate : MonoBehaviour, INetworkObject, INetworkComponent, ISpawnable
{  

    public Triggerable[] triggerables;

    private NetworkContext context;

    public int playerId = 0;

    public bool triggered;

    void Start()
    {
        context = NetworkScene.Register(this);
    }

    private void Awake()
    {
        triggered = false;
    }

    void Update() {
        if (triggered) {
            for (int i=0; i<triggerables.Length; ++i) {
                triggerables[i].beTriggered(this);
            }
            triggered = false;
        }
    }

    void SendTriggers(Collider other) {
        triggered = true;
        context.SendJson(new Message(this.gameObject.transform.position, triggered, playerId));
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag != "Player") return;
        if (playerId == 0) {
            SendTriggers(other);
            playerId = other.gameObject.GetComponent<CollideScript>().Id;
        }
    }

    void OnTriggerExit(Collider other){
        if (other.tag != "Player") return;
        if (other.gameObject.GetComponent<CollideScript>().Id == playerId) {
            playerId = 0;
            SendTriggers(other);

        }
    }

    public NetworkId Id { get; set; } = new NetworkId("13fd688c-20789b8e");


    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();

        if (this.gameObject.transform.position == msg.position) {
            this.triggered = msg.triggered;
            this.playerId = msg.playerId;
        }

    }

    public struct Message
    {
        public Vector3 position;
        public bool triggered;
        public int playerId;
        public Message(Vector3 position, bool triggered, int playerId)
        {
            this.position = position;
            this.triggered = triggered;
            this.playerId = playerId;
        }
    }

    public void OnSpawned(bool local)
    {
    }

}