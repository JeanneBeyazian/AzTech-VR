using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;
using UnityEngine;

public class Teleporting : MonoBehaviour, INetworkObject, INetworkComponent
{   

    private NetworkContext context;
    private Rigidbody body;
    private static int COOLDOWN = 1;  

    public GameObject plane;
    public Camera camera;
    public RenderTexture texture;
    public Material material;
    public Material material2;
    public GameObject portalMesh;

    public struct Message
    {
       public Vector3 position;
    }
    private void Awake()
    {
        body = GetComponent<Rigidbody>();

    }

    

    private void Start()
    {
        // context = NetworkScene.Register(this);
       

    }

    void OnTriggerEnter(Collider other)
    {   
       
        if(other.name == "Manipulator" && PortalWand.can_teleport){
            GameObject otherPortal = null;
            for(int i=0; i< PortalWand.portals.Count; i++)
            {
                
                if (tag == "1" && PortalWand.portals[i].tag == "2"){
                    
                    otherPortal = PortalWand.portals[i].gameObject;

                } else if (tag == "2" && PortalWand.portals[i].tag == "1"){
                    
                    otherPortal = PortalWand.portals[i].gameObject;
                
                }    
            }
            if(otherPortal) {

                Vector3 targetPos = otherPortal.transform.position;
                Camera otherPortalCam = otherPortal.gameObject.GetComponent<Camera>();

                if (otherPortalCam) {

                    material2.mainTexture = this.camera.targetTexture;
                    material.mainTexture = otherPortalCam.targetTexture;
                    this.plane.GetComponent<MeshRenderer>().materials = new Material[] {material};
                    otherPortal.GetComponent<MeshRenderer>().materials = new Material[] {material2};
 
                }

                Debug.Log("Teleport attempt");
                
                targetPos.y -=1f;
                // targetPos.z +=1.5f;
                GameObject righthand = other.gameObject.transform.parent.gameObject;
                GameObject righthandParent = righthand.transform.parent.gameObject;
                GameObject player = righthandParent.transform.parent.gameObject;
                player.transform.position = targetPos;
                PortalWand.can_teleport = false;
                StartCoroutine(tp_time());
            }
        }
       
    }
    private void Update()
    {   
        if(PortalWand.portal_count > 2){
            Destroy(this.gameObject);
            PortalWand.portals.RemoveAt(0);
            PortalWand.portal_count-= 1;
        }
    }

    public NetworkId Id { get; set; }
    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
      
        transform.localPosition = msg.position; // The Message constructor will take the *local* properties of the passed transform.
        // transform.localRotation = msg.transform.rotation;
       
    }

    IEnumerator tp_time(){
        yield return new WaitForSeconds(COOLDOWN);
        PortalWand.can_teleport = true;
    }



}
