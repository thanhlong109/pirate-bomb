using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable 
{
    int Priority { get; }
    GameObject GameObject {  get; }
    NPC_ACTION Action { get; }
}
