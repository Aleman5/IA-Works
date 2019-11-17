using UnityEngine;
using System.Collections;

public class Tank : TankBase
{
    float fitness = 0;
    protected override void OnReset()
    {
        fitness = 1;
    }

    protected override void OnThink(float dt) 
	{
        // Direction to closest mine (normalized!)
        Vector3 dirToGoodMine = GetDirToMine(goodMine);
        Vector3 dirToBadMine = GetDirToMine(badMine);

        // Current tank view direction (it's always normalized)
        Vector3 dir = this.transform.forward;

        // Sets current tank view direction and direction to the mine as inputs to the Neural Network
        inputs[0] = dirToGoodMine.x;
        inputs[1] = dirToGoodMine.z;
        inputs[2] = dirToBadMine.x;
        inputs[3] = dirToBadMine.z;
        inputs[4] = dir.x;
        inputs[5] = dir.z;

        // Think!!! 
        float[] output = brain.Synapsis(inputs);

        SetForces(output[0], output[1], dt);
        Shoot(output[2], dt);
	}
    
    protected override void OnTakeMine(GameObject mine)
    {
        if (mine.tag == "GoodMine")
            fitness *= 2;
        else
            fitness *= 0.5f;
        genome.fitness = fitness;
    }

    public override void OnTakeGoodMine()
    {
        fitness *= 2.0f;
        genome.fitness = fitness;
    }

    public override void OnTakeBadMine()
    {
        fitness *= 0.5f;
        genome.fitness = fitness;
    }
}
