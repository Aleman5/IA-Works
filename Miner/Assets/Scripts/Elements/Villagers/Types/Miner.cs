using UnityEngine;

public class Miner : Villager
{
    [Header("Miner")]
    public float miningSpeed = 1.0f;

    Mine mine = null;

    protected override void Idle()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0.0f)
            TryToFind();
    }

    protected override void Finding()
    {
        mAnimations.IsWalking(false);

        mine = GameManager.Instance.FindClosestMine(transform.position);
        
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

        if (timeLeft <= 0.0f)
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