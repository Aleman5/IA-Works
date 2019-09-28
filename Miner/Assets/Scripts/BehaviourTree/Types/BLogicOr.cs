public class BLogicOr : BLogic
{
    override protected EBState ProcessBNode()
    {
        bool result = false;

        foreach (BNode node in nodes)
        {
            result = node.Evaluate() == EBState.Ok;
            if (result)
                break;
        }

        bState = result ? EBState.Ok : EBState.Fail;

        return bState;
    }
}