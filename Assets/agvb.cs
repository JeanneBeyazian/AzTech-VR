using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agvb : MonoBehaviour
{
    private Rigidbody body;

    private void Awake()
    {
       body = GetComponent<Rigidbody>();
    }


    private void Update()
    {   
        

        if (Input.GetKeyDown(KeyCode.Space))
        {   
            // movement += new Vector3(0f, 5f, 0f);
            body.AddForce(new Vector3(0f, 5f, 0f), ForceMode.Impulse);
        }
    }
}

   