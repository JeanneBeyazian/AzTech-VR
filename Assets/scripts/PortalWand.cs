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
    static public GameObject portal_gun;

    // public static char SPAWN_ENTRY_KEY = 'p';
    public static float COOLDOWN = 2f; 
    private float lastPortalSpawn;

    // moniter item num
    static public bool tp = true;
    static public int portal_count = 0;

    public GameObject portal;
    static public GameObject portal_static;
    static public List<GameObject> portals;

    static public bool one = true;
    static public bool can_teleport = true;
    public bool owner;
    // Start is called before the first frame update
    
    void Awake()
    {
        body = GetComponent<Rigidbody>();
        owner = false;
    }

    private void Start()
    {
        context = NetworkScene.Register(this);
        lastPortalSpawn = Time.time - COOLDOWN;
        portal_gun = gameObject;
        portal_static = portal;
        portals = new List<GameObject>();
    }

    public void Grasp(Hand controller)
    {
        
        grasped = controller;
        owner = true;
    }

    public void Release(Hand controller)
    {
        grasped = null;
    }

    public void UnUse(Hand controller)
    {
       
    }

    public void Use(Hand controller)
    {
        // if (Input.GetKeyDown(SPAWN_ENTRY_KEY.ToString())) {
            if (Time.time > lastPortalSpawn + COOLDOWN) {
               
                Vector3 pos = grasped.transform.position;
                pos.y += 0.75f;
                // GameObject portalProjectileClone  = Instantiate(entryProjectile,  pos, grasped.transform.rotation);
                var portalProjectileClone = NetworkSpawner.SpawnPersistent(this, entryProjectile).GetComponents<MonoBehaviour>().Where(mb => mb is IPortalProjectile).FirstOrDefault() as IPortalProjectile;
                if (portalProjectileClone != null)
                {
                    portalProjectileClone.Attach(grasped);
                }
                // portalProjectileClone.GetComponent<Rigidbody>().velocity = grasped.transform.forward.normalized *SPEED;
                lastPortalSpawn = Time.time;
                
            }

        // }
       
       
    }

    private void Update()
    {   
        if (grasped != null)
        {
            transform.localPosition = grasped.transform.position;
            transform.localRotation = grasped.transform.rotation;

            body.isKinematic = true;

        }
        else
        {
            transform.localPosition = this.gameObject.transform.position;
            transform.localRotation = this.gameObject.transform.rotation;

            body.isKinematic = false;
        }

        if(owner){
            context.SendJson(new Message(transform));
        }
    }
    // Network Unit
    public NetworkId Id { get; set; }= NetworkId.Unique();
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
