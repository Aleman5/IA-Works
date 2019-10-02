using System.Collections.Generic;
using UnityEngine;

public abstract class BNode : MonoBehaviour
{
    public EBState bState;
    public string bName;

    public EBState Evaluate() { return (ProcessBNode()); }

    virtual protected EBState ProcessBNode() { return bState; }
    virtual public void Stop() {  }
    virtual public void Reset() { bState = EBState.None; }
}