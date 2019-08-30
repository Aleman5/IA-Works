using UnityEngine;

public abstract class VillagerBase : Element
{
    [Header("Villager")]
    public int maxMineralsHandle = 0;
    public int mineralsHandling = 0;

    protected Base theBase = null;
    protected bool returnToBase = false;
    protected float timeToObtainEachMat = 2.0f;
    protected float timeToTryFinding = 3.0f;
    protected float timeLeft = 0.0f;
    protected BoxCollider boxCollider;
    protected MinerMovement mMovement;
    protected MinerAction mActions;
    protected MinerAnimations mAnimations;

    virtual protected void Awake()
    {
        theBase = GameManager.Instance.theBase;

        mMovement   = GetComponent<MinerMovement>();
        mActions    = GetComponent<MinerAction>();
        mAnimations = GetComponentInChildren<MinerAnimations>();

        timeLeft = timeToTryFinding;
    }

    protected void Update()
    {
        OnUpdate();
    }

    protected void FixedUpdate()
    {
        
    }

    abstract protected void OnUpdate();

    virtual protected void OnObjectiveFound()
    {

    }
    virtual protected void OnObjectiveNotFound()
    {

    }
    virtual protected void OnBagFull()
    {

    }
    virtual protected void OnBaseCollision()
    {

    }
    virtual protected void OnMineCollision()
    {

    }
    virtual protected void OnMineDestroyed()
    {

    }
    virtual protected void TryToFind()
    {

    }
}