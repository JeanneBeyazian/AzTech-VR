using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;
using UnityEngine;

public class PortalProjectile : MonoBehaviour, INetworkObject, INetworkComponent
{
    // public SphereCollider collider;
    // public Rigidbody body;

    public GameObject portal;

    private NetworkContext context;
    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);

    }
    void Update(){
  
    }
    
    void OnTriggerEnter(Collider other) {

          if (other.gameObject.tag == "Wall"){
            GameObject portalObject = Instantiate(PortalWand.portal_static, this.gameObject.transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(this.gameObject);
            if(PortalWand.one){
                portalObject.tag = "1";
                PortalWand.one = false;
            }else{
                portalObject.tag = "2";
                PortalWand.one = true;
            }
            PortalWand.portals.Add(portalObject);
            PortalWand.portal_count+=1;
            
            
        }
    }
    // Network Unit
    public NetworkId Id { get; set; }
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
      
        transform.localPosition = msg.position; // The Message constructor will take the *local* properties of the passed transform.
        transform.localRotation = msg.rotation;
       
    }
    public struct Message
    {
       public Vector3 position;
       public Quaternion rotation;
    }

}
