using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using Ubiq.Samples;
using System.Linq;
using Ubiq.Messaging;
using UnityEngine;


public class AnimateLever : MonoBehaviour, INetworkObject, INetworkComponent, ISpawnable
{  

    public Animator anim;
    public float cooldown;
    public Triggerable trigger;

    private NetworkContext context;
    public NetworkScene scene;

    private float lastTriggered;
    private bool triggered;
    // Start is called before the first frame update
    void Start()
    {
        context = scene.RegisterComponent(this);
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
    void Update(){

        if(triggered){
            
            trigger.isTriggered = !(trigger.isTriggered);
            
            if(trigger.isTriggered){
                anim.Play("On Lever"); 
            } 
            else{
                anim.Play("Off Lever");
            } 
            lastTriggered = Time.time;
            triggered = false;
        }

        
    }
    void OnTriggerEnter(Collider other){
    }
    void OnTriggerStay(Collider other){
        if (other.tag == "Player" && Input.GetKeyDown("f") && ((lastTriggered+cooldown) < Time.time) ) {
            Debug.Log("Use the Lever");
            triggered = true;
            context.SendJson(new Message(triggered));
        }

    }
    void OnTriggerExit(Collider other){
    }

    // Network Unit

    // public NetworkId Id { get; set; } = NetworkId.Unique();
    public NetworkId Id { get; set; } = new NetworkId("a234-5925-5620-a196");
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        // The Message constructor will take the *local* properties of the passed transform.
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
    public void OnSpawned(bool local)
    {
    }

}