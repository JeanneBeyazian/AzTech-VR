using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using Ubiq.Samples;
using System.Linq;
using Ubiq.Messaging;
using UnityEngine;

public interface IPortalProjectile
{
    void Attach(Hand hand);
}
[RequireComponent(typeof(Rigidbody))]



public class PortalWand : MonoBehaviour, IUseable, IGraspable, INetworkObject, INetworkComponent, ISpawnable
{   
    
    private Rigidbody body;
    private Hand grasped;
    
    private NetworkContext context;
    
    // shooting portal 
    public GameObject entryProjectile;
    public float COOLDOWN = 2f; 
    private float lastPortalSpawn;
    
    public string wandType; 
    // What type of portals this wand shoots
    
    public bool alternatorNextType;
    // If it is an alternator wand, use this boolean to determine next shot

    public bool owner;
    
    void Awake()
    {
        body = GetComponent<Rigidbody>();
        owner = false;
    }

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        context = NetworkScene.Register(this);
        lastPortalSpawn = Time.time - COOLDOWN;
        alternatorNextType = false; // Alternator wands create Entry portal first.
    }

    public void Grasp(Hand controller)
    {
        
        grasped = controller;
        owner = true;
        body.isKinematic = false;
    }

    public void Release(Hand controller)
    {
        body.isKinematic = true;
        grasped = null;
    }

    public void UnUse(Hand controller)
    {
       
    }
    
    private string GetCreatedPortalProjectileTag() {
        if (wandType == "ALTERNATOR") {
            // alternatorNextType = !alternatorNextType;
            // Uncomment this line for flip on shoot.
            return alternatorNextType ? "EXIT" : "ENTRY";
        }
        return wandType;
    }

    public void Use(Hand controller){

        if (Time.time > lastPortalSpawn + COOLDOWN) {
            
            Vector3 pos = grasped.transform.position;
            pos.y += 0.75f;
            // GameObject portalProjectileClone  = Instantiate(entryProjectile,  pos, grasped.transform.rotation);
            
            entryProjectile.tag = GetCreatedPortalProjectileTag();
            // Wand type changes when you shoot if you uncomment in GetCreatedPortalProjectileTag()
            // Or even add the following line here: alternatorNextType = !alternatorNextType;
            // But put it on a keypress! So they can choose what portal they're making.
            // Otherwise, couple them using: So creating a portal changes the type.
            entryProjectile.GetComponent<PortalProjectile>().wandReference = this;
            
            var portalProjectileClone = NetworkSpawner.SpawnPersistent(this, entryProjectile).GetComponents<MonoBehaviour>().Where(mb => mb is IPortalProjectile).FirstOrDefault() as IPortalProjectile;
            if (portalProjectileClone != null)
            {
                portalProjectileClone.Attach(grasped);
            }
            lastPortalSpawn = Time.time;
            
        }

    }

    private void Update()
    {   
        
        if (grasped)
        {
                transform.position = grasped.transform.position;
                transform.rotation = grasped.transform.rotation;
        }
        body.isKinematic = true;
        
        if(owner){
            context.SendJson(new Message(transform));
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
// using System.Collections;
// using System.Collections.Generic;
// using Ubiq.XR;
// using Ubiq.Samples;
// using System.Linq;
// using Ubiq.Messaging;
// using UnityEngine;


// public interface IPortalProjectile
// {
//     void Attach(Hand hand);
// }
// [RequireComponent(typeof(Rigidbody))]
// public class PortalWand : MonoBehaviour, IUseable, IGraspable, INetworkObject, INetworkComponent, ISpawnable
// {   
    
//     private Rigidbody body;
//     private Hand grasped;
    
//     private NetworkContext context;
    
//     // shooting portal 
//     public GameObject entryProjectile;
//     static public GameObject portal_gun;
//     public static float COOLDOWN = 2f; 
//     private float lastPortalSpawn;

//     // moniter item num
//     static public bool tp = true;
//     static public int portal_count = 0;

//     public GameObject portal;
//     static public GameObject portal_static;
//     static public List<GameObject> portals;

//     static public bool one = true;
//     static public bool can_teleport = true;
//     public bool owner;

//     // Start is called before the first frame update
    
//     void Awake()
//     {
//         body = GetComponent<Rigidbody>();
//         owner = false;
//     }

//     private void Start()
//     {
//         body = GetComponent<Rigidbody>();
//         context = NetworkScene.Register(this);
//         lastPortalSpawn = Time.time - COOLDOWN;
//         portal_gun = gameObject;
//         portal_static = portal;
//         portals = new List<GameObject>();
//     }

//     public void Grasp(Hand controller)
//     {
        
//         grasped = controller;
//         owner = true;
//         body.isKinematic = false;
//     }

//     public void Release(Hand controller)
//     {
//         body.isKinematic = true;
//         grasped = null;
//     }

//     public void UnUse(Hand controller)
//     {
       
//     }

//     public void Use(Hand controller)
//     {

//         if (Time.time > lastPortalSpawn + COOLDOWN) {
            
//             Vector3 pos = grasped.transform.position;
//             pos.y += 0.75f;
//             // GameObject portalProjectileClone  = Instantiate(entryProjectile,  pos, grasped.transform.rotation);
//             var portalProjectileClone = NetworkSpawner.SpawnPersistent(this, entryProjectile).GetComponents<MonoBehaviour>().Where(mb => mb is IPortalProjectile).FirstOrDefault() as IPortalProjectile;
//             if (portalProjectileClone != null)
//             {
//                 portalProjectileClone.Attach(grasped);
//             }
//             lastPortalSpawn = Time.time;
            
//         }

       

//     }

//     private void Update()
//     {   
        
//         if (grasped)
//         {
//                 transform.position = grasped.transform.position;
//                 transform.rotation = grasped.transform.rotation;
//         }
//         body.isKinematic = true;
        
//         if(owner){
//             context.SendJson(new Message(transform));
//         }
//     }
//     // Network Unit
//     public NetworkId Id { get; set; }
    
//     public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
//     {
//         var msg = message.FromJson<Message>();
  
//         transform.localPosition = msg.transform.position; // The Message constructor will take the *local* properties of the passed transform.
//         transform.localRotation = msg.transform.rotation;
       
//     }
//     public struct Message
//         {
//             public TransformMessage transform;

//             public Message(Transform transform)
//             {
//                 this.transform = new TransformMessage(transform);
    
//             }
//         }
//     public void OnSpawned(bool local)
//     {
//     }

// }