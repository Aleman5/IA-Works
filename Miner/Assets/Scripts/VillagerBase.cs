using UnityEngine;

public abstract class VillagerBase : MonoBehaviour
{
    virtual protected void Awake()
    {

    }

    protected void Update()
    {
        OnUpdate();
    }

    protected void FixedUpdate()
    {
        
    }

    public void EnemyInSight()
    {

    }

    public void EnemyInAttackRange()
    {
        
    }

    public void StopMoving()
    {

    }

    abstract protected void OnUpdate();

    virtual protected void OnEnemyInSight()
    {

    }
    virtual protected void OnEnemyOutOfSight()
    {

    }
    virtual protected void OnEnemyInAttackRange()
    {

    }
    virtual protected void OnEnemyOutOfAttackRange()
    {

    }
    virtual protected void OnHit()
    {

    }
    virtual protected void OnNoHealth()
    {

    }
    virtual protected void OnPlayerDeath()
    {

    }

    public bool PlayerOnSight()
    {
        return true;
    }

    public bool PlayerOnAttackRange()
    {
        return true;
    }
}