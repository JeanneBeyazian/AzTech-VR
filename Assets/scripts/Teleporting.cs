using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;
using UnityEngine;

public class Teleporting : MonoBehaviour, INetworkObject, INetworkComponent
{   

    public NetworkContext context;
    private Rigidbody body;
    private static int COOLDOWN = 1;
    
    public float portalID;
    
    public static int IDINCREMENT = 0;
    
    
    public GameObject portalManager;
    
    public GameObject linkedPortal; 
    // The portal this is linked to
	
	public Plane plane; 
	// The plane on show
    public Camera camera; 
    // Camera so this portal can see
    public RenderTexture texture; 
    // What this portal sees
    public Material material; 
    // Material to hold RenderTexture

    public GameObject TextName;
    // Contains the text on the portal
    
    public bool isShooter;
    private TextMesh textNameMesh;
    // Controls the text on the portal

    public void UpdateText() {

        if (textNameMesh) {
            textNameMesh.color = (linkedPortal) ? Color.green : Color.red;
            textNameMesh.text = this.tag;
        }
        else {

            textNameMesh = TextName.GetComponent<TextMesh> ();
            
            if (textNameMesh) {
                textNameMesh.color = (linkedPortal) ? Color.green : Color.red;
                textNameMesh.text = this.tag;
            }
        }

        
    }

    
    public void Start() {
        IDINCREMENT += 1;
        this.portalID = IDINCREMENT;
        body = GetComponent<Rigidbody>();

    }
    
    public void OnStart(){

        // inactiveEntryPortals = PortalWand.inactiveEntryPortals;        
        // inactiveExitPortals = PortalWand.inactiveExitPortals;
        // activeEntryPortals = PortalWand.activeEntryPortals;
	    // activeExitPortals = PortalWand.activeExitPortals;

        textNameMesh = TextName.GetComponent<TextMesh> ();

        UpdateText();
    }
    
    
    
    void OnDestroy() { // Linked Portals are unlinked if this one is destroyed.

        PortalManager m = portalManager.GetComponent<PortalManager>();

        if (linkedPortal) {
            
            Teleporting TPLinkedPortal = linkedPortal.GetComponent<Teleporting>();

            // If the destroyed portal had a linked portal,
            // if (isShooter) {
                PortalManager.activeEntryPortals.Remove(portalID);
                PortalManager.activeExitPortals.Remove(TPLinkedPortal.portalID);
  //          }
            // We remove them from being active.
            // Slower than keeping track of indices, but future proof if we want portals to be destructible. 

            
            
            TPLinkedPortal.linkedPortal = null;
            TPLinkedPortal.UpdateText();
            // This might happen implicitly but just in case.
            
            if (this.tag == "ENTRY") {
                if (PortalManager.inactiveExitPortals.Count < PortalManager.MAXIMUM_INACTIVE_PORTALS_OF_ONE_TYPE) {
                    if (!PortalManager.inactiveExitPortals.Contains(TPLinkedPortal.portalID ))
                        m.addPortal(linkedPortal, true);
                    // If it was an entry portal, its exit portal is thrown back into the inactive pool.
                } else {
                    if (linkedPortal) Destroy(linkedPortal);
                    // If there's no space, the linkedPortal is given an index for destruction.
                }
               
            }
            else if (this.tag == "EXIT") {
                if (PortalManager.inactiveEntryPortals.Count < PortalManager.MAXIMUM_INACTIVE_PORTALS_OF_ONE_TYPE) {
                    TPLinkedPortal.material.mainTexture = TPLinkedPortal.camera.targetTexture;
                    // linkedPortal.GetComponent<MeshRenderer>().materials = new Material[] {material};
                    // If it was an exit portal, then its entry portal's graphics are updated
                    if (!PortalManager.inactiveEntryPortals.Contains(TPLinkedPortal.portalID ))
                        m.addPortal(linkedPortal, true);
                    // And if there's space for it, it is returned to the pool
                } else {
                    if (linkedPortal) Destroy(linkedPortal);
                    // Otherwise it would delete other set ups and so it is destroyed instead
                }
            }
        } else {
            if (this.tag == "ENTRY" ) PortalManager.inactiveEntryPortals.Remove(portalID);
            else if (this.tag == "EXIT" ) PortalManager.inactiveExitPortals.Remove(portalID);
            // Unlinked portals are processed just by removing their null index from the list.
        }

        m.SendContext();
    }


    
    public void LinkCameraPortal(GameObject otherPortal) {

        Camera otherPortalCam = otherPortal.gameObject.GetComponent<Camera>();
        // Get the camera of the exit portal
        if (otherPortalCam) {
            material.mainTexture = otherPortalCam.targetTexture;
            this.GetComponent<MeshRenderer>().materials = new Material[] {material};
            // Create a texture using what the other portal sees and apply it to self
        }
        this.linkedPortal = otherPortal;
        // Commit link
    }

    void OnTriggerEnter(Collider other)
    {   

        Debug.Log("Something got hit you know what i mean" + this.tag + other.tag);
        
        UpdateText();
        
        if (other.tag == "DESTROY") {
            Destroy(this.gameObject, 0.3f);
            Destroy(other.gameObject, 0.3f);
        }
        if (!linkedPortal || this.tag != "ENTRY") return;
        if (other.tag == "Player"){ 
            collideScript cs = other.gameObject.GetComponent<collideScript>();
            if (!cs.canTeleport) return;
            cs.canTeleport = false;
            StartCoroutine(BeginPlayerTeleportCooldown(cs));
        }    
          
        Vector3 targetPos = linkedPortal.transform.position;
        targetPos.y -=1f;
            
        Debug.Log("Attempted to teleport");
        other.gameObject.transform.position = targetPos;
        other.gameObject.transform.rotation = linkedPortal.transform.rotation;
    }
    
    IEnumerator BeginPlayerTeleportCooldown(collideScript player){
        yield return new WaitForSeconds(COOLDOWN);
        player.canTeleport = true;
    }
    
    public NetworkId Id { get; set; }
    public void SendContext()
    {
        if (linkedPortal) context.SendJson(new Message(this.gameObject.transform.position, this.linkedPortal.GetComponent<Teleporting>().portalID)); //this.material, this.textNameMesh));
        else context.SendJson(new Message(this.gameObject.transform.position, -1)); //this.material, this.textNameMesh));

    }
    public struct Message
    {
      public Vector3 position;
      public float linkedPortalID;
    //   public Material material;
    //   public TextMesh textNameMesh;
      public Message(Vector3 position, float linkedPortalID)// Material material, TextMesh textNameMesh)
      {
          this.position = position;
          this.linkedPortalID = linkedPortalID;
          //this.material = material;
          //this.textNameMesh = textNameMesh;

      }
    }
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
      
        transform.localPosition = msg.position; 
        
        if (msg.linkedPortalID != -1 && this.linkedPortal == null) {
        
            GameObject[] possibilities = GameObject.FindGameObjectsWithTag(this.tag == "ENTRY" ? "EXIT" : "ENTRY");
            if (possibilities.Length > 0) {
                
                for (int i = 0; i < possibilities.Length; ++i) {
                    Teleporting tp = possibilities[i].GetComponent<Teleporting>();
                    if (tp) {
                        if (tp.portalID == msg.linkedPortalID) {
                            this.linkedPortal = tp.gameObject;
                        }
                    }
                }
                if (linkedPortal) {
                    Teleporting tp = this.linkedPortal.GetComponent<Teleporting>(); ;
                    if (this.tag == "EXIT") {
                        this.LinkCameraPortal(linkedPortal);
                    } else if (this.tag == "ENTRY") {
                        tp.LinkCameraPortal(this.gameObject);
                    }
                    tp.linkedPortal = this.gameObject;
                }
            }
        } else linkedPortal = null;
        
        //this.material = msg.material;
        //this.textNameMesh = msg.textNameMesh;
        UpdateText();
        // The Message constructor will take the *local* properties of the passed transform.
        // transform.localRotation = msg.transform.rotation;
       
    }

}

// using System.Collections;
// using System.Collections.Generic;
// using Ubiq.Messaging;
// using Ubiq.XR;
// using Ubiq.Samples;
// using UnityEngine;

// public class Teleporting : MonoBehaviour, INetworkObject, INetworkComponent
// {   

//     private NetworkContext context;
//     private Rigidbody body;
//     private static int COOLDOWN = 1;  

//     public GameObject plane;
//     public Camera camera;
//     public RenderTexture texture;
//     public Material material;
//     public Material material2;
//     public GameObject portalMesh;

//     public struct Message
//     {
//        public Vector3 position;
//     }
//     private void Awake()
//     {
//         body = GetComponent<Rigidbody>();

//     }

    

//     private void Start()
//     {
//         // context = NetworkScene.Register(this);
       

//     }

//     void OnTriggerEnter(Collider other)
//     {   
       
//         if(other.name == "Manipulator" && PortalWand.can_teleport){
//             GameObject otherPortal = null;
//             for(int i=0; i< PortalWand.portals.Count; i++)
//             {
                
//                 if (tag == "1" && PortalWand.portals[i].tag == "2"){
                    
//                     otherPortal = PortalWand.portals[i].gameObject;

//                 } else if (tag == "2" && PortalWand.portals[i].tag == "1"){
                    
//                     otherPortal = PortalWand.portals[i].gameObject;
                
//                 }    
//             }
//             if(otherPortal) {

//                 Vector3 targetPos = otherPortal.transform.position;
//                 Camera otherPortalCam = otherPortal.gameObject.GetComponent<Camera>();

//                 if (otherPortalCam) {

//                     material2.mainTexture = this.camera.targetTexture;
//                     material.mainTexture = otherPortalCam.targetTexture;
//                     this.plane.GetComponent<MeshRenderer>().materials = new Material[] {material};
//                     otherPortal.GetComponent<MeshRenderer>().materials = new Material[] {material2};
 
//                 }

//                 Debug.Log("Teleport attempt");
                
//                 targetPos.y -=1f;
//                 // targetPos.z +=1.5f;
//                 GameObject righthand = other.gameObject.transform.parent.gameObject;
//                 GameObject righthandParent = righthand.transform.parent.gameObject;
//                 GameObject player = righthandParent.transform.parent.gameObject;
//                 player.transform.position = targetPos;
//                 PortalWand.can_teleport = false;
//                 StartCoroutine(tp_time());
//             }
//         }
       
//     }
//     private void Update()
//     {   
//         if(PortalWand.portal_count > 2){
//             Destroy(this.gameObject);
//             PortalWand.portals.RemoveAt(0);
//             PortalWand.portal_count-= 1;
//         }
//     }


//     public NetworkId Id { get; set; }
    
//     public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
//     {
//         var msg = message.FromJson<Message>();
      
//         transform.localPosition = msg.position; // The Message constructor will take the *local* properties of the passed transform.
//         // transform.localRotation = msg.transform.rotation;
       
//     }

//     IEnumerator tp_time(){
//         yield return new WaitForSeconds(COOLDOWN);
//         PortalWand.can_teleport = true;
//     }



// }