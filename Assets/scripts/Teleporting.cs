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
    
    public static float ACTIVE_LIFETIME = 70f; 
    // If we use the INACTIVE_LIFETIME, not sure if ACTIVE_LIFETIME will come into effect
    // Because active and inactive portals now appear to share the same prefab.
    
    public static float INACTIVE_LIFETIME = 70f; 
	
	public static int MAXIMUM_ACTIVE_PORTAL_PAIRS = 1;
	// Integer for the maximum number of portals we can have active at once.
	
	public static int MAXIMUM_INACTIVE_PORTALS_OF_ONE_TYPE = 1;
	
	public static List<GameObject> inactiveEntryPortals	= new List<GameObject>();
	// List holding all the inactive entry portals
	
	public static List<GameObject> inactiveExitPortals = new List<GameObject>();
	// List holding all the inactive exit portals
	
	public static List<GameObject> activeEntryPortals = new List<GameObject>();
	// List holding all the active entry portals
	
	public static List<GameObject> activeExitPortals = new List<GameObject>();
	// List holding all the active exit portals
    
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

    // private RenderTexture myTexture;
    // private Material myMaterial;
    public GameObject TextName;

    private string type;
    
    public void OnStart(){
        // myMaterial.mainTexture = camera.targetTexture;
        // this.GetComponent<MeshRenderer>().materials = new Material[] {myMaterial};
    }
    public static void addPortal(GameObject portal)
	//public static void addPortal(GameObject portal, String tag) { portal.tag = tag ...
	{
        if (portal.tag == "EXIT") {
            ProcessNewPortal(portal, inactiveExitPortals, inactiveEntryPortals);
        }
        else if (portal.tag == "ENTRY") {
            ProcessNewPortal(portal, inactiveEntryPortals, inactiveExitPortals);
        }
    }
    
    private static void ProcessNewPortal(GameObject portal,
                                         List<GameObject> addToInactive,
                                         List<GameObject> otherInactive) {
                                             
        Debug.Log("Creating portal with tag " + portal.tag);

        if (addToInactive.Count >= MAXIMUM_INACTIVE_PORTALS_OF_ONE_TYPE) {

            Debug.Log("Deleting");
            if (addToInactive.Count != 0) {

                Destroy(addToInactive[0]);
            }
            // Delete the oldest one
        }
        
        addToInactive.Add(portal);
    
        if (addToInactive.Count <= otherInactive.Count) {
            // If there are more or equal inactive portals (of the other kind),
            
            Debug.Log("Attempt linking");
            CheckDestroyOldestPortalPair();
            
            int idx = addToInactive.Count - 1;
            
            Teleporting entryComponent = inactiveEntryPortals[idx].GetComponent<Teleporting>();
                // We can link the two.
            Teleporting exitComponent = inactiveExitPortals[idx].GetComponent<Teleporting>();
            exitComponent.LinkToExitPortal(inactiveEntryPortals[idx]);

            entryComponent.linkedPortal = inactiveExitPortals[idx];

                
            activeExitPortals.Add(inactiveExitPortals[idx]);
            activeEntryPortals.Add(inactiveEntryPortals[idx]);
            // As they are linked an active, they need to be added to the active portals list
                
            inactiveEntryPortals.RemoveAt(idx);
            inactiveExitPortals.RemoveAt(idx);
            // And removed from their original list
        } 
        // else Destroy(portal, INACTIVE_LIFETIME);
    }
    
    // private static void DestroyOldestPortalPairSingle(string portalTag) {
    //     if (activeEntryPortals.Count >= MAXIMUM_ACTIVE_PORTAL_PAIRS && portalTag == "EXIT") {
    //         Destroy(activeEntryPortals[0]);
    //     }
    //     else if (activeExitPortals.Count >= MAXIMUM_ACTIVE_PORTAL_PAIRS && portalTag == "ENTRY") {
    //         Destroy(activeExitPortals[0]);
    //     }
    // }
    

    private static void CheckDestroyOldestPortalPair() {

        if (activeEntryPortals.Count == 0) return;

        if (activeEntryPortals.Count >= MAXIMUM_ACTIVE_PORTAL_PAIRS) {

            GameObject oldLink = activeExitPortals[0];
            Destroy(activeEntryPortals[0]); 
            // Destroy ENTRY first as it renders other's graphics.
            Destroy(oldLink);
        }
    }
    
    void OnDestroy() { // Linked Portals are unlinked if this one is destroyed.
        if (linkedPortal) {
            // If the destroyed portal had a linked portal,
            activeEntryPortals.Remove(this.gameObject);
            activeExitPortals.Remove(linkedPortal);
            // We remove them from being active.
            // Slower than keeping track of indices, but future proof if we want portals to be destructible. 

            Teleporting TPLinkedPortal = linkedPortal.GetComponent<Teleporting>();
            
            TPLinkedPortal.linkedPortal = null;
            // This might happen implicitly but just in case.
            
            if (this.tag == "ENTRY") {
                if (inactiveExitPortals.Count < MAXIMUM_INACTIVE_PORTALS_OF_ONE_TYPE) {
                    addPortal(linkedPortal);
                    // If it was an entry portal, its exit portal is thrown back into the inactive pool.
                } else {
                    Destroy(linkedPortal);
                    // If there's no space, the linkedPortal is given an index for destruction.
                }
               
            }
            else if (this.tag == "EXIT") {
                if (inactiveEntryPortals.Count < MAXIMUM_INACTIVE_PORTALS_OF_ONE_TYPE) {
                    TPLinkedPortal.material.mainTexture = TPLinkedPortal.camera.targetTexture;
                    // linkedPortal.GetComponent<MeshRenderer>().materials = new Material[] {material};
                    // If it was an exit portal, then its entry portal's graphics are updated
                    addPortal(linkedPortal);
                    // And if there's space for it, it is returned to the pool
                } else {
                    Destroy(linkedPortal);
                    // Otherwise it would delete other set ups and so it is destroyed instead
                }
            }
        } else {
            if (this.tag == "ENTRY") inactiveEntryPortals.Remove(this.gameObject);
            else if (this.tag == "EXIT") inactiveExitPortals.Remove(this.gameObject);
            // Unlinked portals are processed just by removing their null index from the list.
        }
    }

    public struct Message
    {
       public Vector3 position;
    }
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }
    
    private void LinkToExitPortal(GameObject exitPortal) {

        Camera otherPortalCam = exitPortal.gameObject.GetComponent<Camera>();
        // Get the camera of the exit portal
        if (otherPortalCam) {
            material.mainTexture = otherPortalCam.targetTexture;
            this.GetComponent<MeshRenderer>().materials = new Material[] {material};
            // Create a texture using what the other portal sees and apply it to self
        }
        this.linkedPortal = exitPortal;
        // Commit link
    }

    private void Start()
    {
        // context = NetworkScene.Register(this);
    }

    void OnTriggerEnter(Collider other)
    {   

        Debug.Log("Something got hit you know what i mean" + this.tag + other.tag);

        
        if (other.tag == "DESTROY") {
            Destroy(this.gameObject);
            Destroy(other.gameObject);
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
            // targetPos.z +=1.5f;
            
            // GameObject righthand = other.gameObject.transform.parent.gameObject;
            // GameObject righthandParent = righthand.transform.parent.gameObject;
            // GameObject player = righthandParent.transform.parent.gameObject;
            // player.transform.position = targetPos; 
            
        Debug.Log("Attempted to teleport");
        other.gameObject.transform.position = targetPos;
        other.gameObject.transform.rotation = linkedPortal.transform.rotation;
        // MIGHT NOT WORK ?
    }
    
    IEnumerator BeginPlayerTeleportCooldown(collideScript player){
        yield return new WaitForSeconds(COOLDOWN);
        player.canTeleport = true;
    }
    
    public NetworkId Id { get; set; }
    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
      
        transform.localPosition = msg.position; 
        // The Message constructor will take the *local* properties of the passed transform.
        // transform.localRotation = msg.transform.rotation;
       
    }


    void Update() {
        TextName.transform.LookAt( Camera.main.transform.position );
        TextName.GetComponent<TextMesh> ().color = Color.red;
        if (linkedPortal) TextName.GetComponent<TextMesh> ().text = " " + this.tag + linkedPortal.name;

        else TextName.GetComponent<TextMesh> ().text = " " + this.tag + " unlinked";
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