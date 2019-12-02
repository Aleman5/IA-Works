using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    CharacterController charControl;
    public float walkSpeed;
    //public GameObject weapon;
    //private Animator anim;

    public Transform bodyTransform;

    [HideInInspector] public bool positionChanged = false;
    uint playerObjectId = 40;
    uint pointsObjectId = 41;

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
            positionChanged = true;
            PlayAnimation();
        }
        if (vert != 0)
        {
            positionChanged = true;
            PlayAnimation();
        }

        Vector3 moveDirSide = transform.right * horiz * walkSpeed * Time.deltaTime;
        Vector3 moveDirForward = transform.forward * vert * walkSpeed * Time.deltaTime;

        charControl.SimpleMove(moveDirSide);
        charControl.SimpleMove(moveDirForward);
    }

    void LateUpdate()
    {
        if (positionChanged)
        {
            MessageManager.Instance.SendEntityInfo(transform.position, transform.rotation, bodyTransform.rotation, false, 0, playerObjectId, ConnectionManager.Instance.clientId);
            positionChanged = false;
        }
    }

    void PlayAnimation()
    {
        //anim.CrossFadeInFixedTime("Walk", 1f);
        //anim.Play("Walk");
        //anim.Play("Walk");
        
    }
}
