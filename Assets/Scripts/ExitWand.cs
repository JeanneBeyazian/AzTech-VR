using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitWand : PortalWand 
{   

    public override void Start(){
        this.wandType = "EXIT";
        base.Start();
    }

}