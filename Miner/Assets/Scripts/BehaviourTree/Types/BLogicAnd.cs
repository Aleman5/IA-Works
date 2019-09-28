public class BLogicAnd : BLogic
{
    override protected EBState ProcessBNode()
    {
        bool result = true;

        foreach (BNode node in nodes)
        {
            result = node.Evaluate() == EBState.Ok;
            if (!result)
                break;
        }

        bState = result ? EBState.Ok : EBState.Fail;

        return bState;
    }
}