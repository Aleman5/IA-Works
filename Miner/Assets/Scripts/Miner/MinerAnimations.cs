using UnityEngine;

public class MinerAnimations : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetSpeed(float speed)
    {
        anim.SetFloat("Speed Normalized", speed);
    }

    public void IsWalking(bool state)
    {
        anim.SetBool("Walking", state);
    }

    public void ReturnBase()
    {
        anim.SetTrigger("Return");
    }

    public void Mining()
    {
        anim.SetTrigger("Mine");
    }
}
