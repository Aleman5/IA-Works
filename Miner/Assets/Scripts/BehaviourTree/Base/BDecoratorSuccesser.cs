public class BDecoratorSuccesser : BDecorator
{
    override protected EBState ProcessBNode()
    {
        nodes[0].Evaluate();
        bState = EBState.Ok;

        return bState;
    }
}