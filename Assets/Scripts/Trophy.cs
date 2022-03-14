using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;


public class Trophy : MonoBehaviour, IGraspable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    gameObject.transform.Rotate(Vector3.up, 1f);
    
        
    }
    
    
    public void Grasp(Hand hand)
    {

        Debug.Log("Grasped");

    }
    
    public void Release(Hand hand)
    {

        Debug.Log("Grasped");

    }
    
}
