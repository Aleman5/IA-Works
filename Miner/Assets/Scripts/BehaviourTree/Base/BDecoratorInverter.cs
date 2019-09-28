public class BDecoratorInverter : BDecorator
{
    override protected EBState ProcessBNode()
    {
        bState = nodes[0].Evaluate();

        if (bState == EBState.Ok)
            bState = EBState.Fail;
        else
            bState = EBState.Ok;

        return bState;
    }
}