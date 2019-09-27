public struct NodeAdy
{
    public Node node;
    public ENodeAdyType type;
}

public struct NodeValue
{
    public int pathValue;
    public int heuristicValue;
    public int value;
    public bool isDanger;
    public bool isRisky;

    public NodeValue(bool isInMud)
    {
        if (!isInMud)
            value = (int)ENodeValueMultipliers.Normal;
        else
            value = (int)ENodeValueMultipliers.Mud;

        pathValue = value;
        heuristicValue = 0;
        isDanger = false;
        isRisky = false;
    }

    public void ResetPathValue()
    {
        pathValue = value;
        heuristicValue = 0;
    }

    public bool IsDanger 
    {
        get { return isDanger; }
        set
        {
            if (isDanger != value)
            {
                isDanger = value;

                if (isDanger)
                    this.value += (int)ENodeValueMultipliers.Danger;
                else
                    this.value -= (int)ENodeValueMultipliers.Danger;
            }
        }
    }

    public bool IsRisky 
    {
        get { return isRisky; }
        set
        {
            if (isRisky != value)
            {
                isRisky = value;
                
                if (isRisky)
                    this.value += (int)ENodeValueMultipliers.Risky;
                else
                    this.value -= (int)ENodeValueMultipliers.Risky;
            }
        }
    }
}