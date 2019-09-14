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
        if (!mine)
        {
            mine = gM.FindClosestMine(transform.position);
            if (!mine)
            {
                OnObjectiveNotFound();
                return;
            }
        }

        Node mineObj = mine.GetAvailableNode();
        if (!mineObj)
        {
            OnObjectiveNotFound();
            return;
        }

        GetPath(mine, mineObj.position);
    }

    protected override void Moving()
    {
        if (path != null)
        {
            mMovement.percReduced = mineralsHandling / maxMineralsHandle;
            mAnimations.SetSpeed(mMovement.percReduced);
            if (mMovement.Move(path[actualPathIndex].position))
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
                Node townCenterObj = myTownCenter.GetAvailableNode();
                if (!townCenterObj)
                {
                    OnObjectiveNotFound();
                    return;
                }

                GetPath(myTownCenter, townCenterObj.position);

                if (path != null)
                    mine.RemoveMiner(this);
            }
            else
                timeLeft = timeToObtainEachMat / miningSpeed;
        }
    }

    public void MineDestroyed()
    {
        switch (GetActualState())
        {
            case (int)States.Moving:
                mine = gM.FindClosestMine(transform.position);
                if (!mine)
                {
                    UIManager.Instance.OnObjectiveNotFound(elementType, EElement.Mine);
                    OnObjectiveNotFound();
                    return;
                }

                Node mineObj = mine.GetAvailableNode();
                if (!mineObj)
                {
                    OnObjectiveNotFound();
                    return;
                }

                GetPath(mine, mineObj.position);
            break;

            case (int)States.Working:
                mine = null;

                Node townCenterObj = myTownCenter.GetAvailableNode();
                if (!townCenterObj)
                {
                    OnObjectiveNotFound();
                    return;
                }

                GetPath(myTownCenter, townCenterObj.position);
                break;
        }
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
                GetPath(objective, objective.GetComponent<Ground>().lastPositionClicked);
                break;

            case EElement.Mine:
                if (objective == mine) return;

                Node mineObj = objective.GetComponent<Mine>().GetAvailableNode();
                if (!mineObj)
                {
                    OnObjectiveNotFound();
                    return;
                }

                GetPath(objective, mineObj.position);
                break;

            case EElement.TownCenter:
                if (GetActualState() == (int)States.Working && mine)
                {
                    mine.RemoveMiner(this);
                    mine = null;
                }

                Node townCenterObj = myTownCenter.GetAvailableNode();
                if (!townCenterObj)
                {
                    OnObjectiveNotFound();
                    return;
                }

                GetPath(objective, townCenterObj.position);
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

            case EElement.TownCenter:
                myTownCenter.DeliverMinerals(ref mineralsHandling);
                OnBaseCollision();
            break;
        }

        objective = null;
    }

    void GetPath(Element objective, Vector3 objectivePos)
    {
        path = gM.pathGenerator.GetPath(
                    gM.nodeGenerator.GetClosestNode(transform.position),
                    gM.nodeGenerator.GetClosestNode(objectivePos),
                    GameManager.Instance.pathfinderType
                );

        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log("Node " + i + ": " + path[i].position);
        }

        if (path != null)
        {
            this.objective = objective;
            OnObjectiveFound();
        }
        else
        {
            UIManager.Instance.OnGoalNotOAttainable();
            OnObjectiveNotFound();
        }
    }
}