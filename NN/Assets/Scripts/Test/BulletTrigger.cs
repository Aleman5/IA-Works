using UnityEngine;

public class BulletTrigger : MonoBehaviour
{
    public TankBase creator;

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "GoodMine")
            creator.OnTakeGoodMine();
        else if (other.transform.tag == "BadMine")
            creator.OnTakeBadMine();

        Destroy(gameObject);
    }
}