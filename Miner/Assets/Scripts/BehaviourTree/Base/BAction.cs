public class BAction : BNode
{
    public delegate EBState ActionNodeDelegate();

    ActionNodeDelegate action;

    public BAction(ActionNodeDelegate action)
    {
        this.action = action;
    }

    override protected EBState ProcessBNode()
    {
        bState = action();
        return bState;
    }
}