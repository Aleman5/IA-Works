using UnityEngine;

public class BWithChild : BNode
{
    [Header("Children")]
    public ushort maxChilds;

    override protected EBState ProcessBNode() { return bState; }
}