using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {

    [SerializeField] private float walkRepeatingTime = 0.8f;
    [SerializeField] private float runRepeatingTime = 0.4f;

    private bool isRuning = false;
    private bool isWalking = false;
    private bool waitForNextStep = false;

    private AudioSource _stepSoundPlayer;
    [SerializeField] private AudioClip[] _randomStepSounds;

    void Start()
    {
        _stepSoundPlayer = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            isWalking = true;
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            isWalking = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRuning = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRuning = false;
        }

        if (isWalking)
        {
            if (isRuning && waitForNextStep == false)
            {
                StartCoroutine(WaitForPlay(runRepeatingTime));
                waitForNextStep = true;
            }
            else if (waitForNextStep == false)
            {
                StartCoroutine(WaitForPlay(walkRepeatingTime));
                waitForNextStep = true;
            }

        }

    }

    IEnumerator WaitForPlay(float delayTime)
    {   
        
        yield return new WaitForSeconds(delayTime);
        /*
        _stepSoundPlayer.clip = _randomStepSounds[Random.Range(0, _randomStepSounds.Length)];
        waitForNextStep = false;
        _stepSoundPlayer.Play();
        */

        int n = Random.Range(1, _randomStepSounds.Length);
        _stepSoundPlayer.clip = _randomStepSounds[n];
        waitForNextStep = false;
        _stepSoundPlayer.PlayOneShot(_stepSoundPlayer.clip);
        // move picked sound to index 0 so it's not picked next time
        _randomStepSounds[n] = _randomStepSounds[0];
        _randomStepSounds[0] = _stepSoundPlayer.clip;
    }
}
