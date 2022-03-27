using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using Ubiq.Samples;
using System.Linq;
using Ubiq.Messaging;
using UnityEngine;


public class LeverAnimation : MonoBehaviour, INetworkObject, INetworkComponent, ISpawnable, IUseable
{  

    public Animator anim;
    public float cooldown;
    private bool grabbed;
    public Triggerable[] triggerables;
    private NetworkContext context;

    private float lastTriggered;
    private bool triggered;
    public bool owner;

    // Start is called before the first frame update
    void Start()
    {
        grabbed = false;
        context = NetworkScene.Register(this);
        anim = GetComponent<Animator>();

        if (cooldown == 0) {
            AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

            foreach(AnimationClip clip in clips){ 
                if (clip.length > cooldown) { 
                    cooldown = clip.length; 
                }
            }
        }

        lastTriggered = Time.time - cooldown;
        anim.Play("Off Lever");
    }
    private void Awake()
    {
        triggered = false;
    }

    public void UnUse(Hand controller){}

    public void Use(Hand controller)
    {
        grabbed = true;
    }

    void Update(){

        if(triggered){

            TriggerAll();

            triggered = false;
        }

    }

    public void TriggerAll() {
    if ((lastTriggered+cooldown) < Time.time) {
     for (int i=0; i<triggerables.Length; ++i) {
                    triggerables[i].beTriggered(this);
                }

                if(triggerables[0].isTriggered){
                    anim.Play("On Lever");
                }
                else{
                    anim.Play("Off Lever");
                }
    }
    }
    
    void OnTriggerStay(Collider other){
        ///&& Input.GetKeyDown("f") 
        if (other.tag == "Player" && Input.GetKeyDown("f") && ((lastTriggered+cooldown) < Time.time)) {
            Debug.Log("Use the Lever");
            grabbed = false;

            context.SendJson(new Message(this.gameObject.transform.position));
            // Maybe copy paste trigger here
            TriggerAll();
            lastTriggered = Time.time; // Make TriggerAll also check CD

        }

    }
    // public NetworkId Id { get; set; }

    public NetworkId Id { get; set; } = new NetworkId("13dd645c-20789b8e");
    // public NetworkComponentId componentId {get; set;}

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();

        Debug.Log("Received" + msg.position + " VS receiver this.game.obj.position " + this.gameObject.transform.position + "/  transform.position" + transform.position);

        if (this.gameObject.transform.position == msg.position) {
            this.triggered = true;
        }

    }



public struct Message
    {
    public Vector3 position;
        public Message(Vector3 position)
        {
            this.position = position;
        }
    }
    public void OnSpawned(bool local)
    {
    }

}