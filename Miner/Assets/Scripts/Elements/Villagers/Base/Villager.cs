public class Villager : VillagerBase
{
    public enum States
    {
        Idle,
        Finding,
        Moving,
        Working,
        Count
    }

    public enum Events
    {
        OnObjectiveFound,
        OnObjectiveNotFound,
        OnBagFull,
        OnBaseCollision,
        OnMineCollision,
        OnMineDestroyed,
        TryToFind,
        Count
    }

    FSM fsm;

    // ===========================================================
    // Inicialization
    // ===========================================================
    private void Start()
    {
        fsm = new FSM((int)States.Count, (int)Events.Count, (int)States.Idle);

                                  // Origin             // Event                         // Destiny
        fsm.SetRelation( (int)States.Idle,     (int)Events.OnObjectiveFound,    (int)States.Moving  );
        fsm.SetRelation( (int)States.Working,  (int)Events.OnObjectiveFound,    (int)States.Moving  );
        fsm.SetRelation( (int)States.Finding,  (int)Events.OnObjectiveFound,    (int)States.Moving  );
        fsm.SetRelation( (int)States.Moving,   (int)Events.OnObjectiveNotFound, (int)States.Idle    );
        fsm.SetRelation( (int)States.Working,  (int)Events.OnObjectiveNotFound, (int)States.Idle    );
        fsm.SetRelation( (int)States.Finding,  (int)Events.OnObjectiveNotFound, (int)States.Idle    );
        fsm.SetRelation( (int)States.Working,  (int)Events.OnBagFull,           (int)States.Moving  );
        fsm.SetRelation( (int)States.Moving,   (int)Events.OnBaseCollision,     (int)States.Finding );
        fsm.SetRelation( (int)States.Moving,   (int)Events.OnMineCollision,     (int)States.Working );
        fsm.SetRelation( (int)States.Moving,   (int)Events.OnMineDestroyed,     (int)States.Finding );
        fsm.SetRelation( (int)States.Working,  (int)Events.OnMineDestroyed,     (int)States.Moving  );
        fsm.SetRelation( (int)States.Idle,     (int)Events.TryToFind,           (int)States.Finding );
        fsm.SetRelation( (int)States.Moving,   (int)Events.TryToFind,           (int)States.Finding );
    }

    // ===========================================================
    // Virtual Methods
    // ===========================================================
    override protected void OnUpdate()
    {
        //Debug.Log((States)fsm.GetState());

        Updating();

        switch (fsm.GetState())
        {
            case (int)States.Idle:
                Idle();
                break;
            case (int)States.Finding:
                Finding();
                break;
            case (int)States.Moving:
                Moving();
                break;
            case (int)States.Working:
                Working();
                break;
        }
    }

    virtual protected void Updating()
    {

    }

    virtual protected void Idle()
    {

    }

    virtual protected void Finding()
    {

    }

    virtual protected void Moving()
    {

    }

    virtual protected void Working()
    {

    }

    void Die()
    {
        
    }

    // ===========================================================
    // Events
    // ===========================================================

    override protected void OnObjectiveFound()
    {
        fsm.SendEvent((int)Events.OnObjectiveFound);
    }

    override protected void OnObjectiveNotFound()
    {
        fsm.SendEvent((int)Events.OnObjectiveNotFound);
    }

    override protected void OnBagFull()
    {
        returnToBase = true;
        mAnimations.ReturnBase();
        fsm.SendEvent((int)Events.OnBagFull);
    }

    protected override void OnBaseCollision()
    {
        returnToBase = false;
        fsm.SendEvent((int)Events.OnBaseCollision);
    }

    protected override void OnMineCollision()
    {
        mAnimations.Mining();
        fsm.SendEvent((int)Events.OnMineCollision);
    }

    protected override void OnMineDestroyed()
    {
        if (fsm.GetState() == (int)States.Working)
        {
            returnToBase = true;
            mAnimations.ReturnBase();
        }
        fsm.SendEvent((int)Events.OnMineDestroyed);
    }

    protected override void TryToFind()
    {
        fsm.SendEvent((int)Events.TryToFind);
    }

    public int GetActualState()
    {
        return fsm.GetState();
    }
}