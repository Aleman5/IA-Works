﻿using UnityEngine;

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

        path = gM.pathGenerator.GetPath(
                    gM.nodeGenerator.GetClosestNode(transform.position),
                    gM.nodeGenerator.GetClosestNode(mine.GetAvailableNode().position),
                    GameManager.Instance.pathfinderType
                );

        if (path != null)
        {
            objective = mine;
            OnObjectiveFound();
        }
        else
            OnObjectiveNotFound();
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
                path = gM.pathGenerator.GetPath(
                    gM.nodeGenerator.GetClosestNode(transform.position),
                    gM.nodeGenerator.GetClosestNode(gM.theBase.GetAvailableNode().position),
                    GameManager.Instance.pathfinderType
                );

                if (path != null)
                {
                    objective = gM.theBase;
                    OnObjectiveFound();
                }
                else
                    OnObjectiveNotFound();
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
                    OnObjectiveNotFound();
            break;

            case EElement.Mine:
                path = gM.pathGenerator.GetPath(
                    gM.nodeGenerator.GetClosestNode(transform.position),
                    gM.nodeGenerator.GetClosestNode(objective.GetComponent<Mine>().GetAvailableNode().position),
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
                    OnObjectiveNotFound();
            break;

            case EElement.Base:
                path = gM.pathGenerator.GetPath(
                    gM.nodeGenerator.GetClosestNode(transform.position),
                    gM.nodeGenerator.GetClosestNode(gM.theBase.transform.position),
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
                    OnObjectiveNotFound();

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
}