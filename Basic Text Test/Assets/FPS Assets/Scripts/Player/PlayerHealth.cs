using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    [SerializeField] int startingHealth = 100;
    [SerializeField] int currentHealth;
    [SerializeField] Slider healthSlider;
    [SerializeField] Image damageImage;
    [SerializeField] AudioClip deathClip;
    [SerializeField] float flashSpeed = 5f;
    [SerializeField] Color flashColour = new Color(1f, 0f, 0f, 0.1f);
    [SerializeField] private Text loseText;
    [SerializeField] private Image dImage;
    private Animator anim;
    private AudioSource playerAudio;
    private PlayerMove playerMovement;
    private PlayerLook playerLook;
    private PlayerJump playerJump;
    private Weapon playerShooting;
    private bool isDead;
    private bool damaged;

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMove>();
        playerLook = GetComponentInChildren<PlayerLook>();
        playerJump = GetComponent<PlayerJump>();
        playerShooting = GetComponentInChildren<Weapon>();
        currentHealth = startingHealth;
    }


    void Update()
    {
        if (damaged)
        {
            //damageImage.color = flashColour;
        }
        else
        {
            //damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;

        if (isDead)
        {
            if (loseText != null)
                loseText.enabled = true;
            //Time.timeScale = 0.1f;
            Invoke("LoadLose", 3f);
        }
    }


    public void TakeDamage(int amount)
    {
        damaged = true;

        currentHealth -= amount;

        healthSlider.value = currentHealth;

        playerAudio.Play();

        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }


    void Death()
    {
        isDead = true;

        playerAudio.clip = deathClip;
        playerAudio.Play();

        playerMovement.enabled = false;
        playerShooting.enabled = false;
        playerLook.enabled = false;
        playerJump.enabled = false;
        dImage.enabled = true;
    }


    void LoadLose()
    {
        //if(Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(1);
    }
}
