public class BDecorator : BWithChild
{
    override protected EBState ProcessBNode() { return bState; }
}