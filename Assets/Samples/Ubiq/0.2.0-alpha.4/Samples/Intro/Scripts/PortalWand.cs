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
    // public GameObject thePortal;
    private Rigidbody body;
    private Hand grasped;
    
    private NetworkContext context;
    // NetworkId INetworkObject.id => new NetworkId(1002);

    // shooting portal 
    public GameObject shootingBall;
    public static char SPAWNKEY = 'p';
    public static float COOLDOWN = 2f; 
    public static float LIFETIME = 45f;
    public static float SPEED = 3f;
    private float lastPortalSpawn;

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
        // var p = NetworkSpawner.SpawnPersistent(this, thePortal).GetComponents<MonoBehaviour>().Where(mb => mb is IPortal).FirstOrDefault() as IPortal;
        // print(0);
        // if (p != null)
        // {
        //     print(1);
        //     p.Attach(controller);
        // }
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

            if (Input.GetKeyDown(SPAWNKEY.ToString())) {
                if (Time.time > lastPortalSpawn + COOLDOWN) {
                    Vector3 pos = grasped.transform.position;
                    pos.y += 1f;
                    GameObject portalProjectileClone  = Instantiate(shootingBall,  pos,
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
