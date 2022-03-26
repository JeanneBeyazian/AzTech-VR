using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using Ubiq.Samples;
using System.Linq;
using Ubiq.Messaging;
using UnityEngine;
public class PortalManager : MonoBehaviour//, INetworkObject, INetworkComponent
{   
//    public NetworkId Id { get; set; }
//    private NetworkContext context;
//
//    public bool owner = false;
//
//    static public List<float> inactiveEntryPortals = new List<float>();//	= PortalWand.inactiveEntryPortals;
//	// List holding all the inactive entry portals
//
//	static public List<float> inactiveExitPortals = new List<float>();//= PortalWand.inactiveExitPortals;
//	// List holding all the inactive exit portals
//
//	static public List<float> activeEntryPortals=new List<float>() ;//= PortalWand.activeEntryPortals;
//	// List holding all the active entry portals
//
//	static public List<float> activeExitPortals=new List<float>() ;//= PortalWand.activeExitPortals;
//	// List holding all the active exit portals
//
//
//    public static float ACTIVE_LIFETIME = 70f;
//    // If we use the INACTIVE_LIFETIME, not sure if ACTIVE_LIFETIME will come into effect
//    // Because active and inactive portals now appear to share the same prefab.
//
//    public static float INACTIVE_LIFETIME = 70f;
//
//	public static int MAXIMUM_ACTIVE_PORTAL_PAIRS = 1;
//	// Integer for the maximum number of portals we can have active at once.
//
//	public static int MAXIMUM_INACTIVE_PORTALS_OF_ONE_TYPE = 1;
//
//    public void Start(){
//        context = NetworkScene.Register(this);
//
//    }
//
//
//    public void addPortal(GameObject portal, bool isShooter)
//	//public static void addPortal(Teleporting portal) {
//	{
//
//        // inactiveEntryPortals = PortalWand.inactiveEntryPortals;
//        // inactiveExitPortals = PortalWand.inactiveExitPortals;
//        // activeEntryPortals = PortalWand.activeEntryPortals;
//	    // activeExitPortals = PortalWand.activeExitPortals;
//        if (!isShooter){
//            StartCoroutine(SmallDelay());
//        }
//        if (portal.tag == "EXIT") {
//            ProcessNewPortal(portal, inactiveExitPortals, inactiveEntryPortals, isShooter);
//        }
//        else if (portal.tag == "ENTRY") {
//            ProcessNewPortal(portal, inactiveEntryPortals, inactiveExitPortals, isShooter);
//        }
//
//        owner = true;
//        if (owner && isShooter) SendContext();
//        owner = false;
//    }
//
//    IEnumerator SmallDelay()
//    {
//        yield return new WaitForSecondsRealtime(1);
//    }
//    public void SendContext() {
//        context.SendJson(new Message(inactiveEntryPortals, inactiveExitPortals, activeEntryPortals, activeExitPortals));
//    }
//
//
//    public static Teleporting PortalSearch(float portalID, string portalTag) {
//        GameObject[] possibleFinds = GameObject.FindGameObjectsWithTag(portalTag);
//        if (possibleFinds.Length > 0) {
//            for (int j = 0; j < possibleFinds.Length; ++j) {
//                Teleporting tp = possibleFinds[j].GetComponent<Teleporting>();
//                if (tp) {
//                    if (tp.portalID == portalID) {
//                        return tp;
//                    }
//                }
//            }
//        }
//
//        return null;
//    }
//
//    private static void ProcessNewPortal(GameObject portal,
//                                         List<float> addToInactive,
//                                         List<float> otherInactive,
//                                         bool isShooter) {
//
//        Debug.Log("Creating portal with tag " + portal.tag);
//
//        if (addToInactive.Count >= MAXIMUM_INACTIVE_PORTALS_OF_ONE_TYPE) {
//
//            Debug.Log("Deleting");
//            if (addToInactive.Count != 0) {
//
//                Destroy(PortalSearch(addToInactive[0], portal.tag).gameObject);
//
//            }
//            // Delete the oldest one
//        }
//
//        Teleporting currentPortal =  portal.GetComponent<Teleporting>();
//
//        if (isShooter) addToInactive.Add(currentPortal.portalID);
//        // For this to work, the shooter has to have added them already.
//
//        if (addToInactive.Count <= otherInactive.Count) {
//            // If there are more or equal inactive portals (of the other kind),
//
//            Debug.Log("Attempt linking");
//
//            int idx = addToInactive.Count - 1;
//
//            CheckDestroyOldestPortalPair(isShooter);
//
//
//                if (isShooter){
//
//                    Teleporting entryComponent = PortalSearch(inactiveEntryPortals[idx], "ENTRY");
//                        // We can link the two.
//                    Teleporting exitComponent = PortalSearch(inactiveExitPortals[idx], "EXIT");
//
//                    exitComponent.LinkCameraPortal(entryComponent.gameObject);
//                    entryComponent.linkedPortal = exitComponent.gameObject;
//
//                    entryComponent.UpdateText();
//                    exitComponent.UpdateText();
//
//                    entryComponent.SendContext();
//                    exitComponent.SendContext();
//
//                    activeExitPortals.Add(inactiveExitPortals[idx]);
//                    activeEntryPortals.Add(inactiveEntryPortals[idx]);
//                    // As they are linked an active, they need to be added to the active portals list
//
//                    inactiveEntryPortals.RemoveAt(idx); // Remove the last one on the list
//                    inactiveExitPortals.RemoveAt(idx);
//                }
//            }
//                // And removed from their original list
//            // }
//            // catch (MissingReferenceException e) {
//            //     Debug.Log(e);
//            // }
//
//        else currentPortal.UpdateText();
//        // else Destroy(portal, INACTIVE_LIFETIME);
//    }
//
//
//    private static void CheckDestroyOldestPortalPair(bool isShooter) {
//
//
//        if (activeEntryPortals.Count == 0) return;
//
//        if (activeEntryPortals.Count >= MAXIMUM_ACTIVE_PORTAL_PAIRS) {
//
//            GameObject oldLink = PortalSearch(activeExitPortals[0], "EXIT").gameObject;
//            Destroy(PortalSearch(activeEntryPortals[0], "ENTRY"));
//            // Destroy ENTRY first as it renders other's graphics.
//            Destroy(oldLink);
//
//        }
//    }
//
//
//    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
//    {
//        var msg = message.FromJson<Message>();
//
//        inactiveEntryPortals = msg.inactiveEntryPortals;
//        inactiveExitPortals = msg.inactiveExitPortals;
//        activeEntryPortals = msg.activeEntryPortals;
//        activeExitPortals = msg.activeExitPortals;
//
//
//    }
//    public struct Message
//        {
//            public List<float> inactiveEntryPortals;
//            public List<float> inactiveExitPortals;
//
//            public List<float> activeEntryPortals;
//
//            public List<float> activeExitPortals;
//
//            public Message(List<float> inactiveEntry, List<float> inactiveExit,
//                                List<float> activeEntry,List<float> activeExit)
//            {
//                this.inactiveEntryPortals= inactiveEntry;
//                this.inactiveExitPortals = inactiveExit;
//                this.activeEntryPortals = activeEntry;
//                this.activeExitPortals = activeExit;
//
//            }
//        }
//    public void OnSpawned(bool local)
//    {
//    }


}