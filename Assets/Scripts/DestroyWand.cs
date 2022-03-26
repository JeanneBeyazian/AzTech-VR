using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWand : PortalWand 
{   

    public override void Start(){
        this.wandType = "DESTROY";
        base.Start();
    }


}