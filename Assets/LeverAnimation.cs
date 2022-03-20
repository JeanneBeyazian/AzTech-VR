using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnimation : MonoBehaviour

{

    public Animator anim;
    public float cooldown;
    public Triggerable trigger;
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
        anim.Play("Off Lever");       
    }

     void OnTriggerStay(Collider other){

         if (other.tag == "Player" && Input.GetKeyDown("f") && ( (lastTriggered+cooldown) < Time.time) ) {
            
            trigger.isTriggered = !(trigger.isTriggered);

            if(trigger.isTriggered) anim.Play("On Lever"); 
            else anim.Play("Off Lever");

            lastTriggered = Time.time;
        
        }

     
    }

}
