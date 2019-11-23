using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    CharacterController charControl;
    public float walkSpeed;
    //public GameObject weapon;
    //private Animator anim;

    void Awake()
    {
        charControl = GetComponent<CharacterController>();
        //anim = weapon.GetComponent<Animator>();
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        if (horiz != 0)
        {
            PlayAnimation();
        }
        if (vert != 0)
        {
            PlayAnimation();
        }

        Vector3 moveDirSide = transform.right * horiz * walkSpeed;
        Vector3 moveDirForward = transform.forward * vert * walkSpeed;

        charControl.SimpleMove(moveDirSide);
        charControl.SimpleMove(moveDirForward);

        
    }

    void PlayAnimation()
    {
        //anim.CrossFadeInFixedTime("Walk", 1f);
        //anim.Play("Walk");
        //anim.Play("Walk");
        
    }
}
