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
    // Sad coupling but will allow the wand to flip on impact if that is preferred.

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
            // this.transform.MoveTowards(transform.position, transform.forward * SPEED;
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
        // Debug.Log("Collision " + this.tag + " - " + other.gameObject.tag);
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
            
            //PortalWand.portal_static.GetComponent<Teleporting>().portalManager = wandReference.portalManager;
            // var portalObject = NetworkSpawner.SpawnPersistent(this.wandReference, PortalWand.portal_static).GetComponents<MonoBehaviour>().Where(mb => mb is Teleporting).FirstOrDefault() as Teleporting;
            // portalObject.gameObject.transform.position = this.gameObject.transform.position;
            // portalObject.gameObject.transform.rotation = normalRotation;

            context.SendJson(new Message(transform, this.tag));
            Destroy(this.gameObject, 0.2f); // Destroy first or it can block itself

            // if (portalObject != null) {
                GameObject portalObject = Instantiate(PortalWand.portal_static, this.gameObject.transform.position, normalRotation);
                // portalObject = NetworkSpawner.Spawn(portalObject);
                // .GetComponents<MonoBehaviour>().Where(mb => mb is IPortalProjectile).FirstOrDefault() as IPortalProjectile;

                Teleporting tpo = portalObject.GetComponent<Teleporting>();

                //tpo.portalManager = wandReference.portalManager;
                // Spawn a portal with that direction
                portalObject.transform.parent = other.gameObject.transform;
                // Attach the portal to what the projectile collided with so they move together
                portalObject.tag = this.tag;

                //bool isShooter = (shooterID == Camera.main.gameObject.transform.parent.transform.parent.GetComponent<collideScript>().uniqueID);
                //tpo.isShooter = isShooter;

                Teleporting.addPortal(portalObject.gameObject);
                // Teleporting.addPortal(portalObject);
                // Let the Portal class know one has been instantiated
                

                if (wandReference && wandReference.wandType == "ALTERNATOR") {
                    wandReference.alternatorNextType = !wandReference.alternatorNextType;
                }
            // }
            // Increases coupling but allows wand to be updated on hit

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
// using System.Collections;
// using System.Collections.Generic;
// using Ubiq.Messaging;
// using Ubiq.XR;
// using Ubiq.Samples;
// using UnityEngine;

// public class PortalProjectile : MonoBehaviour, INetworkObject, INetworkComponent, IPortalProjectile, ISpawnable
// {

//     private Rigidbody body;
//     private Hand grasped;
//     public GameObject portal;
//     private NetworkContext context;

//     public static float LIFETIME = 45f;
//     public static float SPEED = 8f;
//     public bool owner;
//     // Start is called before the first frame update
//     void Start()
//     {
//         context = NetworkScene.Register(this);
        
//     }

//     void Update()
//     {
        
//         if (owner)
//         {
//             if (grasped)
//             {
//                 Vector3 pos = grasped.transform.position;
//                 pos.y += 1f;
//                 transform.position = pos;
//                 transform.rotation = grasped.transform.rotation;
//                 body.isKinematic = false;
//                 body.velocity = grasped.transform.forward.normalized * SPEED;
//                 grasped = null;
//             }
//             context.SendJson(new Message(transform));
//         }
//     }

//     private void Awake()
//     {
//         body = GetComponent<Rigidbody>();
//     }

//     public void Attach(Hand hand)
//     {
//         owner = true;
//         grasped = hand;
//     }

//     async void OnCollisionEnter(Collision other)
//     {

//         if (other.gameObject.tag == "Wall")
//         {
//             Quaternion normalRotation = Quaternion.Euler(0,0,0);
//             Debug.Log("HIT");
//             for (int j = 0; j<other.contacts.Length; ++j) {
//                 // Iterate through the points of contact
//                 if (other.contacts[j].thisCollider == this.gameObject.GetComponent<Collider>()) {
//                     normalRotation = Quaternion.LookRotation(other.contacts[j].normal);
//                     break;
//                 } 

//             }

//             GameObject portalObject = Instantiate(PortalWand.portal_static, this.gameObject.transform.position, normalRotation);
//             portalObject.transform.parent = other.gameObject.transform;
//             Destroy(this.gameObject);
//             if (PortalWand.one)
//             {
//                 portalObject.tag = "1";
//                 PortalWand.one = false;
//             }
//             else
//             {
//                 portalObject.tag = "2";
//                 PortalWand.one = true;
//             }
//             PortalWand.portals.Add(portalObject);
//             PortalWand.portal_count += 1;

//             Destroy(this.gameObject, LIFETIME);
//         }

        
//     }
//     // Network Unit
//     public NetworkId Id { get; set; }
//     public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
//     {
//         var msg = message.FromJson<Message>();

//         transform.position = msg.transform.position; // The Message constructor will take the *local* properties of the passed transform.
//         transform.rotation = msg.transform.rotation;


//     }
//     public struct Message
//     {
//         public TransformMessage transform;
//         public Message(Transform transform)
//         {
//             this.transform = new TransformMessage(transform);
//         }
//     }
//     public void OnSpawned(bool local)
//     {
//     }
// }