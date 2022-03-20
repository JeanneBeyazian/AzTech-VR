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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f")) {

            if (isOn) {
                anim.Play("Off Lever"); 
                isOn = false;
            }
            else {
                anim.Play("On Lever");
                isOn = true;
            }
        
        }
              
    }
}
