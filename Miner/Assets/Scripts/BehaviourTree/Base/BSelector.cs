public class BSelector : BWithChild
{
    int lastIndex = 0;

    override protected EBState ProcessBNode()
    {
        do
        {
            bState = nodes[lastIndex].Evaluate();

            if (bState == EBState.Running || bState == EBState.Ok)
                break;

        } while (++lastIndex < nodes.Count);

        if (lastIndex == nodes.Count)
            lastIndex = 0;

        return bState;
    }

    override public void Reset() { base.Reset(); lastIndex = 0; }
}