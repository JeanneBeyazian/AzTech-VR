using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnimation : MonoBehaviour

{

    public Animator anim;
    public bool isOn;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        isOn = false;
        
    }

     void OnTriggerStay(Collider other){

         if (other.tag == "Player" && Input.GetKeyDown("f")) {
            
            Debug.Log("collision");

            if (isOn) {
                anim.Play("Off Lever"); 
                isOn = false;
            }
            else {
                anim.Play("On Lever");
                isOn = true;
            }
        
        }

     
     //your code
    }

    // Update is called once per frame
    void Update()
    {
        
              
    }
}
