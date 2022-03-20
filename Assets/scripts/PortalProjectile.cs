using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;
using UnityEngine;

public class PortalProjectile : MonoBehaviour, INetworkObject, INetworkComponent, IPortalProjectile, ISpawnable
{
    // public SphereCollider collider;
    private Rigidbody body;
    private Hand grasped;
    public GameObject portal;
    private NetworkContext context;

    public static float LIFETIME = 45f;
    public static float SPEED = 3f;
    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
    }

    void Update(){
        if(grasped){
            
            transform.position = grasped.transform.position;
            transform.rotation = grasped.transform.rotation;
            body.isKinematic = false;
            body.velocity = grasped.transform.forward.normalized * SPEED;
            grasped = null;

        }   
        transform.localPosition = this.gameObject.transform.position;
        transform.localRotation = this.gameObject.transform.rotation;

        context.SendJson(new Message(transform));
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void Attach(Hand hand)
    {
        grasped = hand;
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
            
            Destroy(this.gameObject, LIFETIME);
        }
    }
    // Network Unit
    public NetworkId Id { get; set; }
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
  
        transform.localPosition = msg.transform.position; // The Message constructor will take the *local* properties of the passed transform.
        transform.localRotation = msg.transform.rotation;
       
    }
    public struct Message
        {
            public TransformMessage transform;

            public Message(Transform transform)
            {
                this.transform = new TransformMessage(transform);
    
            }
        }
    public void OnSpawned(bool local)
    {
    }
}
