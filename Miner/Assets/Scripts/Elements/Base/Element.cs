using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Element : MonoBehaviour
{
    [Header("Element")]
    public EElement elementType;

    public virtual void ReactOn(Element objective)
    {

    }
}
