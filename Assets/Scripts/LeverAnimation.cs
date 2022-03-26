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
    public NetworkScene scene;

    private float lastTriggered;
    private bool triggered;
    
    // Start is called before the first frame update
    void Start()
    {
        grabbed = false;
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

    public void UnUse(Hand controller){}


    public void Use(Hand controller)
    {
        grabbed = true;
    }


    async void Update(){

        if(triggered){

            for (int i=0; i<triggerables.Length; ++i) {
                triggerables[i].beTriggered(this);
            }
            
            if(triggerables[0].isTriggered){
                anim.Play("On Lever"); 
            } 
            else{
                anim.Play("Off Lever");
            } 
            lastTriggered = Time.time;
            triggered = false;
        }

        
    }
    void OnTriggerEnter(Collider other){}
    
    void OnTriggerStay(Collider other){
        ///&& Input.GetKeyDown("f") 
        if (other.tag == "Player" && Input.GetKeyDown("f") && ((lastTriggered+cooldown) < Time.time) ) {
            Debug.Log("Use the Lever");
            triggered = true;
            grabbed = false;
            context.SendJson(new Message(triggered));
        }

    }
    void OnTriggerExit(Collider other){
    }

    // Network Unit
    // public NetworkId Id { get; set; } = NetworkId.Unique();
    
    public NetworkId Id { get; set; } = new NetworkId("a236-5925-5620-a196");
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