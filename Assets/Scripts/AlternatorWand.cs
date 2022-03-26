using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternatorWand : PortalWand 
{   

    public override void Start(){
        this.wandType = "ALTERNATOR";
        base.Start();
    }


}