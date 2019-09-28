public class BCondition : BNode
{
    public delegate bool ActionNodeDelegate();

    ActionNodeDelegate action;

    public BCondition(ActionNodeDelegate action)
    {
        this.action = action;
    }

    override protected EBState ProcessBNode()
    {
        if (action())
            bState = EBState.Ok;
        else
            bState = EBState.Fail;

        return bState;
    }
}