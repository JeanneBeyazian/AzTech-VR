using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collideScript : MonoBehaviour
{   
    // private Rigidbody body;
    Vector3 velocity;
    private void Awake()
    {
    //    body = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Platform")
        {

            print("HMM");
        }
        if (other.gameObject.tag =="Object"){
            print("ENTER");
        }
    }
    void OnTriggerStay(Collider other)
    {


    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag =="Object"){
            print("EXIT");
        }
    }
    
        private void Update()
        {   
            
            
            // if (Input.GetKeyDown(KeyCode.Space))
            // {   
            //     // movement += new Vector3(0f, 5f, 0f);
            //     body.AddForce(new Vector3(0f, 6f, 0f), ForceMode.Impulse);
            // }
        }
}
