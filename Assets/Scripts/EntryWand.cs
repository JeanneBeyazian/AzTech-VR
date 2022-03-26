using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryWand : PortalWand 
{   

    public override void Start(){        
        this.wandType = "ENTRY";
        base.Start();
    }


}