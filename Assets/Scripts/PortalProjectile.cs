using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;
using UnityEngine;
using System.Linq;

public class PortalProjectile : MonoBehaviour, INetworkObject, INetworkComponent, IPortalProjectile, ISpawnable
{
    public bool hasDestroyed = false;
    private Rigidbody body;
    private Hand grasped;
    
    
    private NetworkContext context;
    
    public PortalWand wandReference;

    public float LIFETIME = 45f;
    // How long portal projectiles which miss the map will last
    
    public float SPEED = 8f;
    // How fast portal projectiles will travel
    
    public bool owner;

    public bool createdPortal = false;
    // How many times this projectile will "activate"

    public Vector3 portalPosition = Vector3.zero;
    public Quaternion normalRotation = Quaternion.Euler(0,0,0);

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
    }

    void Update() {
            if (createdPortal) {
                portalPosition = this.gameObject.transform.position;
                            context.SendJson(new Message(transform, this.tag, portalPosition, normalRotation));
                            
                            GameObject portalObject = Instantiate(PortalWand.portal_static, portalPosition, normalRotation);
                            
                                            Teleporting tpo = portalObject.GetComponent<Teleporting>();
                            
                                                        // Spawn a portal with that direction
                                            // portalObject.transform.parent = other.gameObject.transform;
                                                        // Attach the portal to what the projectile collided with so they move together
                                            portalObject.tag = this.tag;
                            
                                            Teleporting.addPortal(portalObject.gameObject);
                                                        // Teleporting.addPortal(portalObject);
                                                        // Let the Portal class know one has been instantiated
                            
                                            if (wandReference && wandReference.wandType == "ALTERNATOR") {
                                                wandReference.alternatorNextType = !wandReference.alternatorNextType;
                                            }
                                            

                createdPortal = false;
                Destroy(this.gameObject, 0f);
            }
    }

    void FixedUpdate()
    {


        if (this.GetComponent<Rigidbody>().velocity == Vector3.zero) {
            this.transform.Translate(this.transform.forward * SPEED * Time.smoothDeltaTime);
            // Don't tell server about these: client side protection motion
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
            context.SendJson(new Message(transform, this.tag, Vector3.zero, Quaternion.Euler(0,0,0)));
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

    async void OnCollisionEnter(Collision other)
    {
        if (this.tag == "DESTROY") return ;

        if (other.gameObject.tag == "Wall" && !createdPortal)
        // If we hit a wall
        {
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

            portalPosition = this.gameObject.transform.position;
            context.SendJson(new Message(transform, this.tag, portalPosition, normalRotation));
            
            GameObject portalObject = Instantiate(PortalWand.portal_static, portalPosition, normalRotation);
            
                            Teleporting tpo = portalObject.GetComponent<Teleporting>();
            
                                        // Spawn a portal with that direction
                            // portalObject.transform.parent = other.gameObject.transform;
                                        // Attach the portal to what the projectile collided with so they move together
                            portalObject.tag = this.tag;
            
                            Teleporting.addPortal(portalObject.gameObject);
                                        // Teleporting.addPortal(portalObject);
                                        // Let the Portal class know one has been instantiated
            
                            if (wandReference && wandReference.wandType == "ALTERNATOR") {
                                wandReference.alternatorNextType = !wandReference.alternatorNextType;
                            }
            Destroy(this.gameObject);                
 
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
        this.portalPosition = msg.portalPosition;
        this.normalRotation = msg.normalRotation;
        if (portalPosition != Vector3.zero) createdPortal = true;
    }

    public struct Message
    {
        public TransformMessage transform;
        public string tag;
        public Vector3 portalPosition;
        public Quaternion normalRotation;
        public Message(Transform transform, string tag, Vector3 portalPosition, Quaternion normalRotation)
        {

            this.transform = new TransformMessage(transform);
            this.tag = tag;
            this.portalPosition = portalPosition;
            this.normalRotation = normalRotation;
        }
    }
    public void OnSpawned(bool local)
    {
    }
}