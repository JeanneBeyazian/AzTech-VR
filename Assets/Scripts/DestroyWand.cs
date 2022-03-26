using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWand : PortalWand 
{   

    private void Start(){
        this.wandType = "DESTROY";
        base.Start();
    }


}