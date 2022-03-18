using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPortal : MonoBehaviour
{
    public GameObject movingPortal;
    public static char SPAWNKEY = 'p';
    public static float COOLDOWN = 2f; 

    public static float LIFETIME = 45f;

    public static float SPEED = 3f;

    private float lastPortalSpawn;

    void Start() {
        lastPortalSpawn = Time.time - COOLDOWN;
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(SPAWNKEY.ToString())) {
            if (Time.time > lastPortalSpawn + COOLDOWN) {

                GameObject portalProjectileClone  = Instantiate(movingPortal,  this.gameObject.transform.GetChild(1).transform.position,
                                                                this.gameObject.transform.GetChild(0).transform.rotation);
                // movingPortal = Instantiate(movingPortal,  this.gameObject.transform.GetChild(1).transform.position,  this.gameObject.transform.rotation);
                //movingPortal.rigidbody.velocity = this.gameObject.transform.forward.normalized * SPEED;
                portalProjectileClone.GetComponent<Rigidbody>().velocity = this.gameObject.transform.GetChild(0).transform.forward.normalized *SPEED;
                lastPortalSpawn = Time.time;
                Destroy(portalProjectileClone, LIFETIME);
            }

        }
    }
}