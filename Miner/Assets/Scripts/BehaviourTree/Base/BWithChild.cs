using System.Collections.Generic;
using UnityEngine;

public class BWithChild : BNode
{
    [Header("Children")]
    public ushort maxChilds;
    public List<BNode> nodes;

    override public void Reset()
    {
        base.Reset();

        foreach (BNode node in nodes)
            node.Reset();
    }
}