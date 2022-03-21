using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using Ubiq.Samples;
using System.Linq;
using Ubiq.Messaging;
using UnityEngine;


public class LeverAnimation : MonoBehaviour
{   

    public Animator anim;
    public float cooldown;
    public Triggerable trigger1;
    public Triggerable trigger2;

    private float lastTriggered;

    // Start is called before the first frame update
    void Start()
    {

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

    }
    void Update(){

        
    }
    void OnTriggerStay(Collider other){

         if (other.tag == "Player" && Input.GetKeyDown("f") && ( (lastTriggered+cooldown) < Time.time) ) {

            trigger1.isTriggered = !(trigger1.isTriggered);
            trigger2.isTriggered = !(trigger2.isTriggered);

            if(trigger1.isTriggered) anim.Play("On Lever"); 
            else anim.Play("Off Lever");

            lastTriggered = Time.time;

        }


    }


}