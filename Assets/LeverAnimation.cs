using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using Ubiq.Samples;
using System.Linq;
using Ubiq.Messaging;
using UnityEngine;


public class LeverAnimation : MonoBehaviour, INetworkObject, INetworkComponent, ISpawnable
{  

    public Animator anim;
    public float cooldown;
    public Triggerable trigger1;
    public Triggerable trigger2;

    private NetworkContext context;

    private float lastTriggered;
    
    public bool owner;
    public bool triggered;
    // Start is called before the first frame update
    void Start()
    {
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
        owner = false;
        triggered = false;
    }
    void Update(){

        if(owner && triggered){
            
            trigger1.isTriggered = !(trigger1.isTriggered);
            trigger2.isTriggered = !(trigger2.isTriggered);
            
            if(trigger1.isTriggered){
                anim.Play("On Lever"); 
            } 
            else{
                anim.Play("Off Lever");
            } 
            triggered = !(triggered);
            lastTriggered = Time.time;
            context.SendJson(new Message(triggered, owner));
        }
        
    }
    void OnTriggerEnter(Collider other){
        owner = true;
        print("Enter");
    }
    void OnTriggerStay(Collider other){
        if (other.tag == "Player" && Input.GetKeyDown("f") && ( (lastTriggered+cooldown) < Time.time) ) {
            triggered = true;
        }

    }
    void OnTriggerExit(Collider other){
        owner = false;
    }

    // Network Unit
    public NetworkId Id { get; set; }
    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
  
        // The Message constructor will take the *local* properties of the passed transform.
        this.triggered = msg.triggered;
        this.owner = msg.owner;
        trigger1.isTriggered = msg.triggered;
        trigger2.isTriggered = msg.triggered;

    }
    public struct Message
    {
        public bool triggered;
        public bool owner;
        public Message( bool triggered, bool owner)
        {
            this.triggered = triggered;
            this.owner = owner;
        }
    }
    public void OnSpawned(bool local)
    {
    }

}