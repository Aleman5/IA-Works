public class BSequence : BWithChild
{
    int lastIndex = 0;

    override protected EBState ProcessBNode()
    {
        if (bState == EBState.None)
            lastIndex = 0;

        do
        {
            bState = nodes[lastIndex].Evaluate();

            if (bState == EBState.Running)
                break;

        } while (++lastIndex < nodes.Count);

        if (lastIndex == nodes.Count)
            lastIndex = 0;

        return bState;
    }
}