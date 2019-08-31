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
        /*
            Aca tengo que hacer el movimiento del player con el Path que consegui
        */

        if (returnToBase)
        {
            
            mMovement.percReduced = mineralsHandling / maxMineralsHandle;
            mAnimations.SetSpeed(mMovement.percReduced);
            mMovement.Move(theBase.transform.position);

            return;
        }

        if (mine)
        {
            Vector3 minePos = mine.transform.position;
            minePos.y = 0.0f;
            mMovement.percReduced = mineralsHandling / maxMineralsHandle;
            mAnimations.SetSpeed(mMovement.percReduced);
            mMovement.Move(minePos);

            return;
        }

        TryToFind();
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
        switch (objective.elementType)
        {
            case EElement.Ground:
                path = gM.pathGenerator.GetPath(
                    gM.nodeGenerator.GetClosestNode(transform.position),
                    gM.nodeGenerator.GetClosestNode(objective.GetComponent<Ground>().lastPositionClicked),
                    EPathfinderType.Star
                );

                if (path != null)
                    OnObjectiveFound();
                else
                    OnObjectiveNotFound();
            break;

            case EElement.Mine:

            break;
        }
    }

    void OnTriggerEnter(Collider other)
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
    }
}