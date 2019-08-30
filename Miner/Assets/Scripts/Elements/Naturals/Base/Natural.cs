using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Natural : Element
{
    [Header("Natural")]
    public float materialsHandle;

    protected float materialsLeft;

    virtual protected void Awake()
    {
        materialsLeft = materialsHandle;
    }
}
