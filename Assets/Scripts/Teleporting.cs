using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Samples;
using UnityEngine;

public class Teleporting : MonoBehaviour, INetworkObject, INetworkComponent
{   
    private Material privateMaterial;

    private NetworkContext context;
    private Rigidbody body;
    private static float COOLDOWN = 1f;
    
    public static float ACTIVE_LIFETIME = 70f; 
    
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
    public Camera portalCamera; 
    public Material activeMaterial;
    public Material inactiveMaterial;
// Material to hold RenderTexture

    public GameObject TextName;
    // Contains the text on the portal
    
    private TextMesh textNameMesh;
    // Controls the text on the portal

    public void UpdateText() {
        textNameMesh = TextName.GetComponent<TextMesh> ();
        if (textNameMesh) {
            textNameMesh = TextName.GetComponent<TextMesh> ();
        }
        if (textNameMesh) {
            textNameMesh.color = (linkedPortal) ? Color.green : Color.red;
            textNameMesh.text = this.tag;
        }

        transform.GetChild(3).GetComponent<MeshRenderer>().materials = new Material []
            {(linkedPortal) ? activeMaterial : inactiveMaterial};
    }


    public void Awake() {
            body = GetComponent<Rigidbody>();
            privateMaterial = new Material(Shader.Find("Specular"));
            portalCamera.targetTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            privateMaterial.mainTexture = portalCamera.targetTexture;
            transform.GetChild(1).GetComponent<MeshRenderer>().materials = new Material [] {privateMaterial};
            }

    
    public void Start(){
        context = NetworkScene.Register(this);
        textNameMesh = TextName.GetComponent<TextMesh> ();
        UpdateText();
        context.SendJson(new Message(this.gameObject.transform.position));
    }
    public static void addPortal(GameObject portal)
	{
        if (portal.tag == "EXIT") {
            ProcessNewPortal(portal, inactiveExitPortals, inactiveEntryPortals);
        }
        else if (portal.tag == "ENTRY") {
            ProcessNewPortal(portal, inactiveEntryPortals, inactiveExitPortals);
        }
    }

void Update() {
        float timePulse = Mathf.Abs(0.05f * Mathf.Sin(Time.time)) + 0.9f;
        // Values from PortalWand.portal_static
        float xmul = PortalWand.portal_static.transform.localScale.x;
        float ymul = PortalWand.portal_static.transform.localScale.y;
        Vector3 vec = new Vector3(xmul * timePulse, ymul * timePulse, 0.008f);
        transform.localScale = vec;
    }
    
    private static void ProcessNewPortal(GameObject portal,
                                         List<GameObject> addToInactive,
                                         List<GameObject> otherInactive) {
                                             
        Debug.Log("Creating portal with tag " + portal.tag);
        
        

        if (addToInactive.Count >= MAXIMUM_INACTIVE_PORTALS_OF_ONE_TYPE) {

            if (addToInactive.Count != 0) {

                Destroy(addToInactive[0]);
            }
            // Delete the oldest one
        }
        
        addToInactive.Add(portal);
    
        if (addToInactive.Count <= otherInactive.Count) {
            // If there are more or equal inactive portals (of the other kind)
            
            CheckDestroyOldestPortalPair();
        
            int idx = addToInactive.Count - 1;
            
            Teleporting entryComponent = inactiveEntryPortals[idx].GetComponent<Teleporting>();
            // We can link the two.
            Teleporting exitComponent = inactiveExitPortals[idx].GetComponent<Teleporting>();
            
            entryComponent.LinkCameraPortal(inactiveExitPortals[idx]);
            exitComponent.linkedPortal = inactiveEntryPortals[idx];
            
            entryComponent.UpdateText();
            exitComponent.UpdateText();
                
            activeExitPortals.Add(inactiveExitPortals[idx]);
            activeEntryPortals.Add(inactiveEntryPortals[idx]);
            // As they are linked an active, they need to be added to the active portals list
                
            inactiveEntryPortals.RemoveAt(idx);
            inactiveExitPortals.RemoveAt(idx);
            // And removed from their original list
        } 
        else portal.GetComponent<Teleporting>().UpdateText();
    }
    
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

            Teleporting TPLinkedPortal = linkedPortal.GetComponent<Teleporting>();
            
            TPLinkedPortal.linkedPortal = null;
            TPLinkedPortal.UpdateText();
            
            if (this.tag == "ENTRY") {
                if (inactiveExitPortals.Count < MAXIMUM_INACTIVE_PORTALS_OF_ONE_TYPE) {
                        addPortal(linkedPortal);
                    // If it was an entry portal, its exit portal is thrown back into the inactive pool.
                } else {
                    if (linkedPortal) Destroy(linkedPortal);
                    // If there's no space, the linkedPortal is given an index for destruction.
                }
               
            }
            else if (this.tag == "EXIT") {
                if (inactiveEntryPortals.Count < MAXIMUM_INACTIVE_PORTALS_OF_ONE_TYPE) {

                    TPLinkedPortal.privateMaterial = new Material(Shader.Find("Specular"));

                    TPLinkedPortal.portalCamera.targetTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);

                    TPLinkedPortal.privateMaterial.mainTexture = TPLinkedPortal.portalCamera.targetTexture;

                    MeshRenderer r = linkedPortal.transform.GetChild(1).GetComponent<MeshRenderer>();
                    if (r) r.materials = new Material[] {TPLinkedPortal.privateMaterial};

                    // If it was an exit portal, then its entry portal's graphics are updated
                    addPortal(linkedPortal);
                    // And if there's space for it, it is returned to the pool
                } else {
                    if (linkedPortal) Destroy(linkedPortal);
                    // Otherwise it would delete other set ups and so it is destroyed instead
                }
            }
        } else {
            if (this.tag == "ENTRY") inactiveEntryPortals.Remove(this.gameObject);
            else if (this.tag == "EXIT") inactiveExitPortals.Remove(this.gameObject);
            // Unlinked portals are processed just by removing their null index from the list.
        }
    }

    private void LinkCameraPortal(GameObject otherPortal) {
        this.linkedPortal = otherPortal;
        this.transform.GetChild(1).GetComponent<MeshRenderer>().materials = new Material[] {linkedPortal.GetComponent<Teleporting>().privateMaterial};
    }

    void OnTriggerEnter(Collider other)
    {   

        Debug.Log("Enter Portal" + this.tag + other.tag);
        
        UpdateText();
        
        if (other.tag == "DESTROY") {
            Destroy(this.gameObject, 0.1f);
            Destroy(other.gameObject, 0.1f);
            return;
        }

        if (!linkedPortal || this.tag == "EXIT") {
            TeleportColliderToPortal(other, this);
            return;
        }

        TeleportColliderToPortal(other, linkedPortal.GetComponent<Teleporting>());

    }

    private void TeleportColliderToPortal(Collider other, Teleporting destination) {
        CollideScript cooldownScript = other.gameObject.GetComponent<CollideScript>();

        if (cooldownScript) {
            if (!cooldownScript.canTeleport) return;
            if (other.tag == "Player") {
                other.gameObject.transform.rotation = Quaternion.Euler(0, destination.transform.eulerAngles.y , 0);
            } else {
                other.gameObject.transform.rotation = Quaternion.Euler(destination.transform.eulerAngles.x,
                                                                       destination.transform.eulerAngles.y,
                                                                       destination.transform.eulerAngles.z);
            }
            other.gameObject.transform.position = destination.transform.position + other.gameObject.transform.forward * 2;
            // Slight offset in teleport destination
            cooldownScript.canTeleport = false;
            StartCoroutine(BeginColliderTeleportCooldown(cooldownScript));
        } else {
            other.gameObject.transform.rotation = Quaternion.Euler(destination.transform.eulerAngles.x,
                                                                   destination.transform.eulerAngles.y,
                                                                   destination.transform.eulerAngles.z);
            other.gameObject.transform.position = destination.transform.position + other.gameObject.transform.forward * 2;
            // Slight offset in teleport destination

            Rigidbody possibleVelocity = other.gameObject.GetComponent<Rigidbody>();
            if (possibleVelocity) {
                float speed = possibleVelocity.velocity.magnitude;
                if (speed > 0) {
                    possibleVelocity.velocity = possibleVelocity.transform.forward.normalized * speed;
                }
            } // May require a context update though (?)
        }
    }
    
    IEnumerator BeginColliderTeleportCooldown(CollideScript coll){
        yield return new WaitForSeconds(COOLDOWN);
        coll.canTeleport = true;
    }
    
    public NetworkId Id { get; set; }
    public struct Message
    {
       public Vector3 position;
       public Message(Vector3 p) {
           position = p;
       }
       
    }
    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
     
    }

}