using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalProjectile : MonoBehaviour
{
    // public SphereCollider collider;
    // public Rigidbody body;

    public GameObject portal;

    // public GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        // manager = PortalManager.Instance;

    }
    void Update(){
  
    }
    
    void OnTriggerEnter(Collider other) {

          if (other.gameObject.tag == "Wall"){
            GameObject portalObject = Instantiate(PortalWand.portal_static, this.gameObject.transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(this.gameObject);
            if(PortalWand.one){
                portalObject.tag = "1";
                PortalWand.one = false;
            }else{
                portalObject.tag = "2";
                PortalWand.one = true;
            }
            PortalWand.portals.Add(portalObject);
            PortalWand.portal_count+=1;
            
            
        }
    }

}
