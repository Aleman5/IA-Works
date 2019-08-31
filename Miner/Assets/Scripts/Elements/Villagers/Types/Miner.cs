using UnityEngine;

public class Miner : Villager
{
    [Header("Miner")]
    public float miningSpeed = 1.0f;

    Mine mine = null;

    protected override void Idle()
    {
        /*timeLeft -= Time.deltaTime;

        if (timeLeft <= 0.0f)
            TryToFind();*/
    }

    protected override void Finding()
    {
        mAnimations.IsWalking(false);

        mine = gM.FindClosestMine(transform.position);
        
        if (mine)
        {
            OnObjectiveFound();
            mAnimations.IsWalking(true);
        }
        else
        {
            timeLeft = timeToTryFinding;
            OnObjectiveNotFound();
        }
    }

    protected override void Moving()
    {
        if (path != null)
        {
            mMovement.percReduced = mineralsHandling / maxMineralsHandle;
            mAnimations.SetSpeed(mMovement.percReduced);
            if (mMovement.Move(path[actualPathIndex].transform.position))
            {
                actualPathIndex++;
                if (actualPathIndex == path.Count)
                {
                    actualPathIndex = 0;
                    path = null;
                    ObjectiveReached();
                }
            }
        }
    }

    protected override void Working()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0.0f && mine)
        {
            mine.RemoveMaterial();
            mineralsHandling++;

            if (mineralsHandling == maxMineralsHandle)
            {
                mine.RemoveMiner(this);
                OnBagFull();
            }
            else
                timeLeft = timeToObtainEachMat / miningSpeed;
        }
    }

    public void MineDestroyed()
    {
        OnMineDestroyed();
    }

    public override void ReactOn(Element objective)
    {
        if (this.gameObject == objective.gameObject)
        {
            path = null;
            OnObjectiveNotFound();
        }

        switch (objective.elementType)
        {
            case EElement.Ground:
                path = gM.pathGenerator.GetPath(
                    gM.nodeGenerator.GetClosestNode(transform.position),
                    gM.nodeGenerator.GetClosestNode(objective.GetComponent<Ground>().lastPositionClicked),
                    EPathfinderType.Star
                );

                if (path != null)
                {
                    this.objective = objective;
                    OnObjectiveFound();
                }
                else
                    OnObjectiveNotFound();
            break;

            case EElement.Mine:

            break;
        }
    }

    void ObjectiveReached()
    {
        switch (objective.elementType)
        {
            case EElement.Mine:
                mine = objective.GetComponent<Mine>();
                mine.AddMiner(this);
                OnMineCollision();
            break;

            case EElement.Base:
                theBase.DeliverMinerals(ref mineralsHandling);
                OnBaseCollision();
            break;
        }

        objective = null;
    }

    /*void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Base" && returnToBase)
        {
            theBase.DeliverMinerals(ref mineralsHandling);
            OnBaseCollision();
        }
        else if (other.tag  == "Mine" && !returnToBase)
        {
            mine.AddMiner(this);
            OnMineCollision();
        }
    }*/
}