using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Weapon : MonoBehaviour {

    private Animator anim;
    private AudioSource _AudioSource;

    [SerializeField] private float range = 100f;                            
    [SerializeField] private int bulletsPerMag = 30;                       
    [SerializeField] private int bulletsLeft = 200;                        
    [SerializeField] private int currentBullets;                           

    [SerializeField] private Text ammoText;
    [SerializeField] private Transform shootPoint;                         
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private GameObject bulletImpact;
    //[SerializeField] private GameObject thisWeapon;

    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip shootSound;

    [SerializeField] private float fireRate = 0.1f;                         
    [SerializeField] private int damage = 20;

    float fireTimer;                                                    

    private bool isReloading;                                               

    private void OnEnable()
    {
        UpdadateAmmoText();
        
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        _AudioSource = GetComponent<AudioSource>();
        currentBullets = bulletsPerMag;
        UpdadateAmmoText();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (currentBullets > 0)
                Fire();                                                     
            else if(bulletsLeft > 0)
                DoReload();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentBullets < bulletsPerMag && bulletsLeft > 0)
                DoReload();
        }

        if (fireTimer < fireRate)
            fireTimer += Time.deltaTime;                                    
    }

    private void FixedUpdate()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        isReloading = info.IsName("Recharge");                                
    }

    private void Fire()
    {                                                                      
        if (fireTimer < fireRate || currentBullets <= 0 || isReloading) return; 

        RaycastHit hit;                                                    

        if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        {
            // HIT SOMEONE
        }

        anim.CrossFadeInFixedTime("Shot", 0.01f);                           
        muzzleFlash.Play();                                                 
        PlayShootSound();                                                   

        currentBullets--;
        UpdadateAmmoText();

        fireTimer = 0.0f;                                                  
    }

    public void Reload()
    {
        if (bulletsLeft <= 0) return;                                       

        int bulletsToLoad = bulletsPerMag - currentBullets;                 
        int bulletsToDeduct = (bulletsLeft >= bulletsToLoad) ? bulletsToLoad : bulletsLeft;

        bulletsLeft -= bulletsToDeduct;
        currentBullets += bulletsToDeduct;

        UpdadateAmmoText();
    }

    private void DoReload()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if (isReloading) return;

        anim.CrossFadeInFixedTime("Recharge", 0.01f);
    }

    private void PlayShootSound()
    {
        _AudioSource.PlayOneShot(shootSound);                               
    }

    private void UpdadateAmmoText()
    {
        //ammoText.text = currentBullets + " / " + bulletsLeft;
    }
}