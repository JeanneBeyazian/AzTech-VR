using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collideScript : MonoBehaviour
{   

    Vector3 velocity;
    void Update()
    {   

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag =="Object"){
            print("ENTER");
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag =="Object"){
            print("STAY");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag =="Object"){
            print("EXIT");
        }
    }
}
