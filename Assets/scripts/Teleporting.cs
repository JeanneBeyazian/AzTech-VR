using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;
using UnityEngine;

public class Teleporting : MonoBehaviour, INetworkObject, INetworkComponent
{   

    // public Transform teleportTarget;
    private NetworkContext context;
    private Rigidbody body;

    // NetworkId INetworkObject.id => new NetworkId(1001);

    public struct Message
    {
       public Vector3 position;
    }
    private void Awake()
    {
        body = GetComponent<Rigidbody>();

    }

    public NetworkId Id { get; set; } 

    private void Start()
    {
        // context = NetworkScene.Register(this);
        // Material[] materials = {inactive, inactive};
        // this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials = materials;
        // this.gameObject.GetComponent<MeshRenderer>().materials[0] = inactive;
        // this.gameObject.GetComponent<MeshRenderer>().materials[1] = inactive;     

    }

    void OnTriggerEnter(Collider other)
    {   
       
        // if (teleportTarget) {
            
        //     Vector3 targetPos = teleportTarget.transform.position;
        //     targetPos.x -=1f;
        //     targetPos.z +=1.5f;
        //     GameObject righthand = other.gameObject.transform.parent.gameObject;
        //     GameObject righthandParent = righthand.transform.parent.gameObject;
        //     GameObject player = righthandParent.transform.parent.gameObject;
        //     player.transform.position = targetPos;
        // }
        GameObject otherPortal = null;
        for(int i=0; i< PortalWand.portals.Count; i++)
        {
            if (tag == "1" && PortalWand.portals[i].tag == "2"){

                otherPortal = PortalWand.portals[i].gameObject;

            }else if (tag == "2" && PortalWand.portals[i].tag == "1"){
                
                otherPortal = PortalWand.portals[i].gameObject;
            
            }    
        }
        if(otherPortal){
            Vector3 targetPos = otherPortal.transform.position;
            targetPos.x -=1f;
            targetPos.z +=1.5f;
            GameObject righthand = other.gameObject.transform.parent.gameObject;
            GameObject righthandParent = righthand.transform.parent.gameObject;
            GameObject player = righthandParent.transform.parent.gameObject;
            // player.transform.position = targetPos;

        }
    }
    private void Update()
    {   
        print(PortalWand.portal_count);


        if(PortalWand.portal_count > 2){
            Destroy(this.gameObject);
            PortalWand.portal_count-= 1;
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
      
        transform.localPosition = msg.position; // The Message constructor will take the *local* properties of the passed transform.
        // transform.localRotation = msg.transform.rotation;
       
    }



}
