using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaldPirateNPC : PirateNPC
{

    public override void HandleBomb(GameObject bomb)
    {
        MoveToBomb(bomb, () => {
            Debug.Log("reach bomb");
        
        });
    }

    
}
