using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnimation : MonoBehaviour

{

    public Animator anim;

    public float cooldown;
    public Triggerable trigger;
    private float lastTriggered;
    public bool isOn;

    public bool isActivated;

    // Start is called before the first frame update
    void Start()
    {

        anim = GetComponent<Animator>();
        isOn = false; 

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

     void OnTriggerStay(Collider other){

         if (other.tag == "Player" && Input.GetKeyDown("f") && ( (lastTriggered+cooldown) < Time.time) ) {

            isOn = !isOn; 
            
            trigger.isTriggered = !(trigger.isTriggered);

            if (!isOn) {
                anim.Play("Off Lever"); 
            }
            else {
                anim.Play("On Lever");
            }

            lastTriggered = Time.time;
        
        }

     
    }


    public bool getIsOn(){
        return isOn;
    }
}
