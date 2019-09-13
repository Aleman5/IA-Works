using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Natural : Element
{
    [Header("Natural")]
    public ushort maxWorkers;
    public float materialsHandle;
    public float materialsLeft = 0;

    virtual protected void Awake()
    {
        materialsLeft = materialsHandle;
    }
}
