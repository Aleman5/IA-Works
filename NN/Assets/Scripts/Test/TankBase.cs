using UnityEngine;
using System.Collections;

public class TankBase : MonoBehaviour
{
    [Header("Variables")]
    public float Speed = 10.0f;
    public float RotSpeed = 20.0f;
    public float maxDist = 10.0f;
    public float fireRate = 2.0f;

    [Header("Objetcs")]
    public GameObject bulletPrefab;
    public Transform bulletOrigin;

    protected Genome genome;
	protected NeuralNetwork brain;
    protected GameObject nearMine;
    protected GameObject goodMine;
    protected GameObject badMine;
    protected float[] inputs;

    float timeLeft = 0.0f;
    GameObject bullet;
    Rigidbody bulletRb;

    // Sets a brain to the tank
    public void SetBrain(Genome genome, NeuralNetwork brain)
    {
        this.genome = genome;
        this.brain = brain;
        inputs = new float[brain.InputsCount];
        OnReset();
    }

    // Used by the PopulationManager to set the closest mine
    public void SetNearestMine(GameObject mine)
    {
        nearMine = mine;
    }

    public void SetGoodNearestMine(GameObject mine)
    {
        goodMine = mine;
    }

    public void SetBadNearestMine(GameObject mine)
    {
        badMine = mine;
    }

    protected bool IsGoodMine()
    {
        return nearMine.tag == "GoodMine";
    }

    protected Vector3 GetDirToMine(GameObject mine)
    {
        return (mine.transform.position - this.transform.position).normalized;
    }
    
    protected bool IsCloseToMine(GameObject mine)
    {
        return (this.transform.position - nearMine.transform.position).sqrMagnitude <= 2.0f;
    }

    protected void SetForces(float leftForce, float rightForce, float dt)
    {
        // Tank position
        Vector3 pos = this.transform.position;

        // Use the outputs as the force of both tank tracks
        float rotFactor = Mathf.Clamp((rightForce - leftForce), -1.0f, 1.0f);

        // Rotate the tank as the rotation factor
        this.transform.rotation *= Quaternion.AngleAxis(rotFactor * RotSpeed * dt, Vector3.up);

        // Move the tank in current forward direction
        pos += this.transform.forward * Mathf.Abs(rightForce + leftForce) * 0.5f * Speed * dt;

        // Sets current position
        this.transform.position = pos;
    }

	// Think is called once per frame
	public void Think(float dt) 
	{
        OnThink(dt);

        /*if(IsCloseToMine(nearMine))
        {
            OnTakeMine(nearMine);
            // Move the mine to a random position in the screen
            PopulationManager.Instance.RelocateMine(nearMine);
        }*/
	}

    private void Awake()
    {
        timeLeft = fireRate;
        bullet = Instantiate(bulletPrefab, bulletOrigin.position, transform.rotation);
        bullet.SetActive(false);
        bulletRb = bullet.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        timeLeft -= Time.fixedDeltaTime;
        
        if (timeLeft <= 0.0f)
        {
            timeLeft = fireRate;

            bullet.SetActive(true);
            bullet.transform.position = bulletOrigin.position;
            bullet.transform.rotation = bulletOrigin.rotation;
        }
    }

    protected virtual void OnThink(float dt)
    {

    }

    protected virtual void OnTakeMine(GameObject mine)
    {
    }

    public virtual void OnTakeGoodMine()
    {
    }

    public virtual void OnTakeBadMine()
    {
    }

    protected virtual void OnReset()
    {

    }
}
