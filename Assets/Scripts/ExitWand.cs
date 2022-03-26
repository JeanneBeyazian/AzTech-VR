using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitWand : PortalWand 
{   

    private void Start(){
        this.wandType = "EXIT";
        base.Start();
    }


}