using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternatorWand : PortalWand 
{   

    private void Start(){
        this.wandType = "ALTERNATOR";
        base.Start();
    }


}