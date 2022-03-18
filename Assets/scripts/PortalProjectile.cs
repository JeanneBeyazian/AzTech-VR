using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalProjectile : MonoBehaviour
{
    // public SphereCollider collider;
    // public Rigidbody body;

    public GameObject portal;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    void OnTriggerEnter(Collider other) {
        print(2);
        if (other.gameObject.tag == "Wall"){
            print(1);
            Destroy(this.gameObject);

            GameObject portalObject = Instantiate(portal, this.gameObject.transform.position, Quaternion.Euler(0, 0, 0));
            //Quaternion.Euler(90, 90, 90) * other.transform.rotation
        }
    }
}
