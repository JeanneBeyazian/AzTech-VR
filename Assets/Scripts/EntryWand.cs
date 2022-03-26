using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryWand : PortalWand 
{   

    private void Start(){        
        this.wandType = "ENTRY";
        base.Start();
    }


}