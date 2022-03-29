using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public GameObject respawnPoint;

    public float additionalYHeight = 1f;
    
    void OnTriggerEnter(Collider other) {
        
        if (other.tag != "Player") return;
        
            Debug.Log("New Waypoint saved");
            Vector3 newPos = this.gameObject.transform.position;
            newPos.y += additionalYHeight;
            respawnPoint.gameObject.transform.position = newPos;
    }
}