using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;
using UnityEngine;
using System.Linq;

public class PortalProjectile : MonoBehaviour, INetworkObject, INetworkComponent, IPortalProjectile, ISpawnable
{
    public int shooterID;
    private Rigidbody body;
    private Hand grasped;
    
    private NetworkContext context;
    
    public PortalWand wandReference;

    public float LIFETIME = 45f;
    // How long portal projectiles which miss the map will last
    
    public float SPEED = 8f;
    // How fast portal projectiles will travel
    
    public bool owner;
    
    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
        
    }

    void FixedUpdate()
    {
        if (this.GetComponent<Rigidbody>().velocity == Vector3.zero) {

            this.transform.Translate(this.transform.forward * SPEED * Time.smoothDeltaTime);
        }

        if (owner)
        {
            if (grasped)
            {
                Vector3 pos = grasped.transform.position;
                pos.y += 1f;
                transform.position = pos;
                transform.rotation = grasped.transform.rotation;
                body.isKinematic = false;
                body.velocity = grasped.transform.forward.normalized * SPEED;
                grasped = null;
            }
            context.SendJson(new Message(transform, this.tag));
        }
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void Attach(Hand hand)
    {
        owner = true;
        grasped = hand;
    }

    void OnDestroy(){
        context.SendJson(new Message(transform, this.tag));
    }
        

    async void OnCollisionEnter(Collision other)
    {
        context.SendJson(new Message(transform, this.tag));

        if (this.tag == "DESTROY") return ;

        
        if (other.gameObject.tag == "Wall")
        // If we hit a wall
        {

            Quaternion normalRotation = Quaternion.Euler(0,0,0);
            // Initialise an empty normal rotation for the instantiated object to face
            
            Collider ourCollider = this.gameObject.GetComponent<Collider>();
            for (int j = 0; j < other.contacts.Length; ++j) {
                // For each collision the other GameObject is experiencing
                if (other.contacts[j].thisCollider == ourCollider) {
                    // If it's colliding with us, set normalRotation to the normal to that collision
                    normalRotation = Quaternion.LookRotation(other.contacts[j].normal);
                    break;
                } 
            }

            context.SendJson(new Message(transform, this.tag));
            Destroy(this.gameObject, 0.2f); // Destroy first or it can block itself

            GameObject portalObject = Instantiate(PortalWand.portal_static, this.gameObject.transform.position, normalRotation);

            Teleporting tpo = portalObject.GetComponent<Teleporting>();

            // Spawn a portal with that direction
            portalObject.transform.parent = other.gameObject.transform;
            // Attach the portal to what the projectile collided with so they move together
            portalObject.tag = this.tag;

            Teleporting.addPortal(portalObject.gameObject);
            // Teleporting.addPortal(portalObject);
            // Let the Portal class know one has been instantiated
            
            if (wandReference && wandReference.wandType == "ALTERNATOR") {
                wandReference.alternatorNextType = !wandReference.alternatorNextType;
            }

        }
    }
    
    // Network Unit
    public NetworkId Id { get; set; } = NetworkId.Unique();

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();

        transform.position = msg.transform.position; // The Message constructor will take the *local* properties of the passed transform.
        transform.rotation = msg.transform.rotation;
        this.tag = msg.tag;


    }
    public struct Message
    {
        public TransformMessage transform;
        public string tag;
        public Message(Transform transform, string tag)
        {

            this.transform = new TransformMessage(transform);
            this.tag = tag;
        }
    }
    public void OnSpawned(bool local)
    {
    }
}