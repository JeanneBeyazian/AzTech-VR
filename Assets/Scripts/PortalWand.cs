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
    public GameObject portal;
    static public GameObject portal_static;

    public GameObject portal_gun;

    public float COOLDOWN = 2f;
    private float lastPortalSpawn;
    
    public string wandType;

    public bool owner;
    public bool alternatorNextType = false;


    void Awake()
    {
        body = GetComponent<Rigidbody>();
        owner = false;
    }


    public virtual void Start()
    {
        context = NetworkScene.Register(this);
        lastPortalSpawn = Time.time - COOLDOWN;
        portal_gun = gameObject;
        portal_static = portal;
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
    
    private string GetCreatedPortalProjectileTag() {
        if (wandType == "ALTERNATOR") {
            // alternatorNextType = !alternatorNextType;
            // Uncomment this line for flip on shoot.
            return alternatorNextType ? "EXIT" : "ENTRY";
        }
        return wandType;
    }

    public void Use(Hand controller)
    {

        if (Time.time > lastPortalSpawn + COOLDOWN) {
            // if (grapsed)
            Vector3 pos = grasped.transform.position;
            pos.y += 0.75f;
            entryProjectile.tag = GetCreatedPortalProjectileTag();
            PortalProjectile pp = entryProjectile.GetComponent<PortalProjectile>(); 
            pp.wandReference = this;
            
            var portalProjectileClone = NetworkSpawner.SpawnPersistent(this, entryProjectile).GetComponents<MonoBehaviour>()
                .Where(mb => mb is IPortalProjectile).FirstOrDefault() as IPortalProjectile;
            
            if (portalProjectileClone != null) portalProjectileClone.Attach(grasped);
        
            lastPortalSpawn = Time.time;
            
        }
       
    }

    public void DestroyPortal(Teleporting portal) {
        context.SendJson(new Message(transform, portal.portalId, portal.gameObject.tag));

        Destroy(portal.gameObject);
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
            context.SendJson(new Message(transform, -1, ""));
        }
    }

    // Network Unit
    public NetworkId Id { get; set; }
    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();

        if (msg.portalIdToDestroy != -1 && msg.portalTagToDestroy != "") {
            Teleporting foundPortal = Teleporting.PortalSearch(msg.portalIdToDestroy, msg.portalTagToDestroy);
            if (foundPortal) Destroy(foundPortal.gameObject);
        } else {
            transform.localPosition = msg.transform.position; // The Message constructor will take the *local* properties of the passed transform.
            transform.localRotation = msg.transform.rotation;
        }

    }
    public struct Message
        {
            public TransformMessage transform;

            public int portalIdToDestroy;
            public string portalTagToDestroy;

            public Message(Transform transform, int portalIdToDestroy, string portalTagToDestroy)
            {
                this.transform = new TransformMessage(transform);
                this.portalIdToDestroy = portalIdToDestroy;
                this.portalTagToDestroy = portalTagToDestroy;
            }
        }
    public void OnSpawned(bool local)
    {
        owner = local;
    }

}