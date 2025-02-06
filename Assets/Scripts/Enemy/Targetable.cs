using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Targetable : MonoBehaviour
{
    [SerializeField]protected int priority;
    protected GameObject targetObject = null;
}
