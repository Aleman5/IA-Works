using UnityEngine;

public class BulletTrigger : MonoBehaviour
{
    public float speed = 5.0f;
    public TankBase creator;

    private void FixedUpdate()
    {
        transform.Translate(transform.forward * speed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "GoodMine")
        {
            creator.OnTakeGoodMine();
            PopulationManager.Instance.RelocateMine(other.gameObject);
        }
        else if (other.transform.tag == "BadMine")
        {
            creator.OnTakeBadMine();
            PopulationManager.Instance.RelocateMine(other.gameObject);
        }

        gameObject.SetActive(false);
    }
}