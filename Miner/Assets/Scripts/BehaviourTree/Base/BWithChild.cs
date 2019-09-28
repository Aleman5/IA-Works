using System.Collections.Generic;
using UnityEngine;

public class BWithChild : BNode
{
    [Header("Children")]
    public ushort maxChilds;
    public List<BNode> nodes;

    override protected EBState ProcessBNode() { return bState; }
}