using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using Ubiq.Samples;
using System.Linq;
using Ubiq.Messaging;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
public class PortalWand : MonoBehaviour, IUseable, IGraspable, INetworkObject, INetworkComponent
{   
    
    private Rigidbody body;
    private Hand grasped;
    
    private NetworkContext context;
    // NetworkId INetworkObject.id => new NetworkId(1002);
    
    // shooting portal 

    public GameObject entryProjectile;
    static public GameObject portal_gun;

    public static char SPAWN_ENTRY_KEY = 'p';
    public static float COOLDOWN = 2f; 
    public static char SPAWN_EXIT_KEY = 'l';

    public static float LIFETIME = 45f;

    public static float SPEED = 3f;
    private float lastPortalSpawn;


    // moniter item num
    static public bool tp = true;
    static public int portal_count = 0;

    public GameObject portal;
    static public GameObject portal_static;
    static public List<GameObject> portals;

    static public bool one = true;
    static public bool can_teleport = true;
    // Start is called before the first frame update
    public NetworkId Id { get; set; }
    void Awake()
    {
        body = GetComponent<Rigidbody>();
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
 
    }

    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
      
        transform.localPosition = msg.position; // The Message constructor will take the *local* properties of the passed transform.
        // transform.localRotation = msg.transform.rotation;
       
    }

    private void Update()
    {   
        if (grasped != null)
        {
            transform.localPosition = grasped.transform.position;
            
            Message message;
            message.position = transform.localPosition;
            context.SendJson(message);

            body.isKinematic = true;

            if (Input.GetKeyDown(SPAWN_ENTRY_KEY.ToString())) {
                if (Time.time > lastPortalSpawn + COOLDOWN) {
                    Vector3 pos = grasped.transform.position;
                    pos.y += 0.75f;
                    GameObject portalProjectileClone  = Instantiate(entryProjectile,  pos,
                                                                    grasped.transform.rotation);

                    portalProjectileClone.GetComponent<Rigidbody>().velocity = grasped.transform.forward.normalized *SPEED;

                    lastPortalSpawn = Time.time;
                    Destroy(portalProjectileClone, LIFETIME);
                }

            }
        }
        else
        {
            transform.localPosition = this.gameObject.transform.position;
            
            Message message;
            message.position = transform.localPosition;
            context.SendJson(message);

            body.isKinematic = false;
        }
    }

    public struct Message
    {
       public Vector3 position;
    }

}
